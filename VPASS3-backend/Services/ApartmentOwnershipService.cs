using VPASS3_backend.Context;
using VPASS3_backend.DTOs.ApartmentOwnerships;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using VPASS3_backend.Utils;
using Microsoft.EntityFrameworkCore;

namespace VPASS3_backend.Services
{
    public class ApartmentOwnershipService : IApartmentOwnershipService
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserContextService _userContext;
        private readonly IAuditLogService _auditLogService;

        public ApartmentOwnershipService(
            AppDbContext dbContext,
            IUserContextService userContext,
            IAuditLogService auditLogService)
        {
            _dbContext = dbContext;
            _userContext = userContext;
            _auditLogService = auditLogService;
        }

        public async Task<ResponseDto> CreateAsync(CreateApartmentOwnershipDto dto)
        {
            var apartment = await _dbContext.Apartments
                .Include(a => a.Zone)
                .Include(a => a.Ownerships)
                .ThenInclude(o => o.Person)
                .FirstOrDefaultAsync(a => a.Id == dto.IdApartment);

            if (apartment == null)
                return new ResponseDto(404, message: "Departamento no encontrado.");

            var zone = apartment.Zone;
            if (zone == null)
                return new ResponseDto(404, message: "Zona no encontrada para el departamento.");

            if (_userContext.UserRole != "SUPERADMIN" &&
                (!_userContext.EstablishmentId.HasValue || _userContext.EstablishmentId.Value != zone.EstablishmentId))
            {
                return new ResponseDto(403, message: "No tienes permisos para asignar un propietario en este establecimiento.");
            }

            //Se obtiene el propietario activo más reciente (sin fecha de término)
            var activeOwner = await ApartmentUtils.GetActiveOwnershipAsync(_dbContext, dto.IdApartment);

            if (activeOwner != null)
            {
                return new ResponseDto(409, message:
                    $"El departamento ya tiene un propietario activo: {activeOwner.Person.Names} {activeOwner.Person.LastNames}");
            }

            var person = await _dbContext.Persons.FindAsync(dto.IdPerson);
            if (person == null)
                return new ResponseDto(404, message: "Persona no encontrada.");

            var newOwnership = new ApartmentOwnership
            {
                IdApartment = dto.IdApartment,
                IdPerson = dto.IdPerson,
                StartDate = TimeHelper.GetSantiagoTime()
            };

            _dbContext.ApartmentOwnerships.Add(newOwnership);
            await _dbContext.SaveChangesAsync();

            await _auditLogService.LogManualAsync(
                action: $"Asignó propietario {person.Names} {person.LastNames} al departamento {apartment.Name ?? $"ID {apartment.Id}"}",
                email: _userContext.UserEmail,
                role: _userContext.UserRole,
                userId: _userContext.UserId ?? 0,
                endpoint: "/apartmentownership/create",
                httpMethod: "POST",
                statusCode: 201
            );

            return new ResponseDto(201, newOwnership, "Propietario asignado correctamente.");
        }

        public async Task<ResponseDto> EndCurrentOwnershipAsync(EndApartmentOwnershipDto dto)
        {
            // Se obtiene el departamento con sus zonas y propietarios (incluyendo datos de la persona)
            var apartment = await _dbContext.Apartments
                .Include(a => a.Zone)
                .Include(a => a.Ownerships)
                .ThenInclude(o => o.Person)
                .FirstOrDefaultAsync(a => a.Id == dto.IdApartment);

            // Si el departamento no existe, se retorna error 404
            if (apartment == null)
                return new ResponseDto(404, message: "Departamento no encontrado.");

            // Se verifica que la zona asociada al departamento esté presente
            var zone = apartment.Zone;
            if (zone == null)
                return new ResponseDto(400, message: "Zona no encontrada para el departamento.");

            // Si el usuario no es SUPERADMIN, se valida que pertenezca al establecimiento correcto
            if (_userContext.UserRole != "SUPERADMIN" &&
                (!_userContext.EstablishmentId.HasValue || _userContext.EstablishmentId.Value != zone.EstablishmentId))
            {
                return new ResponseDto(403, message: "No tienes permisos para editar propiedad en este establecimiento.");
            }

            // Se busca el propietario activo más reciente (sin fecha de término)
            var activeOwner = apartment.Ownerships
                .OrderByDescending(o => o.Id)
                .FirstOrDefault(o => o.EndDate == null);

            // Si no hay ningún propietario activo, se retorna un mensaje indicando que ya está desocupado
            if (activeOwner == null)
                return new ResponseDto(409, message: "Este departamento no tiene un propietario activo.");

            // Se establece la fecha de término del propietario activo con la hora actual de Santiago
            activeOwner.EndDate = TimeHelper.GetSantiagoTime();

            // Se guarda el cambio en la base de datos
            await _dbContext.SaveChangesAsync();

            // Se registra el evento en el log de auditoría
            await _auditLogService.LogManualAsync(
                action: $"Se dio término el uso de {activeOwner.Person.Names} {activeOwner.Person.LastNames} en el departamento {apartment.Name ?? $"ID {apartment.Id}"}",
                email: _userContext.UserEmail,
                role: _userContext.UserRole,
                userId: _userContext.UserId ?? 0,
                endpoint: "/apartmentownership/end",
                httpMethod: "PUT",
                statusCode: 200
            );

            // Se retorna la respuesta con el propietario actualizado
            return new ResponseDto(200, activeOwner, "Propiedad finalizada correctamente.");
        }


        public async Task<ResponseDto> GetAllAsync()
        {
            var query = _dbContext.ApartmentOwnerships
                .Include(o => o.Apartment).ThenInclude(a => a.Zone)
                .Include(o => o.Person)
                .AsQueryable();

            if (_userContext.UserRole != "SUPERADMIN")
            {
                if (!_userContext.EstablishmentId.HasValue)
                    return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                query = query.Where(o => o.Apartment.Zone.EstablishmentId == _userContext.EstablishmentId.Value);
            }

            var list = await query.ToListAsync();
            return new ResponseDto(200, list, "Propiedades obtenidas correctamente.");
        }

        public async Task<ResponseDto> GetByIdAsync(int id)
        {
            var entry = await _dbContext.ApartmentOwnerships
                .Include(o => o.Apartment).ThenInclude(a => a.Zone)
                .Include(o => o.Person)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (entry == null)
                return new ResponseDto(404, message: "Registro no encontrado.");

            if (_userContext.UserRole != "SUPERADMIN" &&
                (!_userContext.EstablishmentId.HasValue || entry.Apartment.Zone.EstablishmentId != _userContext.EstablishmentId.Value))
            {
                return new ResponseDto(403, message: "No tienes permiso para ver este registro.");
            }

            return new ResponseDto(200, entry);
        }

        public async Task<ResponseDto> DeleteAsync(int id)
        {
            var entry = await _dbContext.ApartmentOwnerships
                .Include(o => o.Apartment).ThenInclude(a => a.Zone)
                .Include(o => o.Person)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (entry == null)
                return new ResponseDto(404, message: "Registro no encontrado.");

            if (_userContext.UserRole != "SUPERADMIN" &&
                (!_userContext.EstablishmentId.HasValue || entry.Apartment.Zone.EstablishmentId != _userContext.EstablishmentId.Value))
            {
                return new ResponseDto(403, message: "No tienes permiso para eliminar este registro.");
            }

            _dbContext.ApartmentOwnerships.Remove(entry);
            await _dbContext.SaveChangesAsync();

            await _auditLogService.LogManualAsync(
                action: $"Eliminó registro de propiedad de {entry.Person.Names} {entry.Person.LastNames}",
                email: _userContext.UserEmail,
                role: _userContext.UserRole,
                userId: _userContext.UserId ?? 0,
                endpoint: "/apartmentownership/delete",
                httpMethod: "DELETE",
                statusCode: 200
            );

            return new ResponseDto(200, message: "Registro eliminado correctamente.");
        }
    }
}