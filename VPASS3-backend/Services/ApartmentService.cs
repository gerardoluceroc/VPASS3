using VPASS3_backend.Context;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.DTOs.Apartments;

namespace VPASS3_backend.Services
{
    public class ApartmentService : IApartmentService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;
        private readonly IAuditLogService _auditLogService;

        public ApartmentService(AppDbContext context, IUserContextService userContext, IAuditLogService auditLogService)
        {
            _context = context;
            _userContext = userContext;
            _auditLogService = auditLogService;
        }

        public async Task<ResponseDto> GetAllApartmentsAsync()
        {
            try
            {
                var sections = await _context.Apartments
                    .Where(zs => !zs.IsDeleted)
                    .Include(zs => zs.Zone)
                    .Include(zs => zs.Ownerships)
                        .ThenInclude(o => o.Person)
                    .ToListAsync();

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    sections = sections
                        .Where(zs => _userContext.CanAccessApartment(zs))
                        .ToList();
                }

                return new ResponseDto(200, sections, "Departamentos obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllApartmentsAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener los departamentos.");
            }
        }

        public async Task<ResponseDto> GetApartmentByIdAsync(int id)
        {
            try
            {
                var section = await _context.Apartments
                    .Include(zs => zs.Zone)
                    .Include(zs => zs.Ownerships)
                        .ThenInclude(o => o.Person)
                    .FirstOrDefaultAsync(zs => zs.Id == id && !zs.IsDeleted);

                if (section == null)
                    return new ResponseDto(404, message: "Departamento no encontrado.");

                if (!_userContext.CanAccessApartment(section))
                    return new ResponseDto(403, message: "No tienes permiso para acceder a este departamento.");

                return new ResponseDto(200, section, "Departamento obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetApartmentByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener el departamento.");
            }
        }

        public async Task<ResponseDto> CreateApartmentAsync(ApartmentDto dto)
        {
            try
            {
                var zone = await _context.Zones
                    .FirstOrDefaultAsync(z => z.Id == dto.IdZone);

                if (zone == null)
                    return new ResponseDto(404, message: "Zona asociada no encontrada.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != zone.EstablishmentId)
                {
                    return new ResponseDto(403, message: "No tienes permiso para crear departamentos en esta zona.");
                }

                //  Validar si ya existe un departamento con el mismo nombre (no eliminado)
                var exists = await _context.Apartments
                    .AnyAsync(zs => zs.Name == dto.Name && zs.IdZone == dto.IdZone && !zs.IsDeleted);

                if (exists)
                    return new ResponseDto(409, message: "Ya existe un departamento con ese nombre en esta zona.");

                var apartment = new Apartment
                {
                    Name = dto.Name,
                    IdZone = dto.IdZone
                };

                _context.Apartments.Add(apartment);
                await _context.SaveChangesAsync();

                var message = $"Se creó el departamento '{apartment.Name}' que es parte de la zona '{zone.Name}'";

                await _auditLogService.LogManualAsync(
                    action: message,
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: "/Apartment/create",
                    httpMethod: "POST",
                    statusCode: 201
                );

                return new ResponseDto(201, apartment, message: "Departamento creado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateApartmentAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al crear el departamento.");
            }
        }

        public async Task<ResponseDto> UpdateApartmentAsync(int id, ApartmentDto dto)
        {
            try
            {
                var apartment = await _context.Apartments
                    .Include(zs => zs.Zone)
                    .FirstOrDefaultAsync(zs => zs.Id == id);

                if (apartment == null)
                    return new ResponseDto(404, message: "Departamento no encontrado.");

                if (!_userContext.CanAccessApartment(apartment))
                    return new ResponseDto(403, message: "No tienes permiso para editar este departamento.");

                var zone = await _context.Zones.FirstOrDefaultAsync(z => z.Id == dto.IdZone);
                if (zone == null)
                    return new ResponseDto(404, message: "Zona asociada no encontrada.");

                //  Validación de duplicado (si cambia de nombre o zona)
                bool isDuplicate = await _context.Apartments.AnyAsync(zs =>
                    zs.Id != id &&
                    zs.IdZone == dto.IdZone &&
                    zs.Name == dto.Name &&
                    !zs.IsDeleted);

                if (isDuplicate)
                    return new ResponseDto(409, message: "Ya existe un departamento con ese nombre en esta zona.");

                // Actualizar campos
                apartment.Name = dto.Name;
                apartment.IdZone = dto.IdZone;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, apartment, "Departamento actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateApartmentAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al actualizar el departamento.");
            }
        }

        public async Task<ResponseDto> DeleteApartmentAsync(int id)
        {
            try
            {
                var apartment = await _context.Apartments
                    .Include(zs => zs.Zone)
                    .FirstOrDefaultAsync(zs => zs.Id == id && !zs.IsDeleted);

                if (apartment == null)
                    return new ResponseDto(404, message: "Subzona no encontrada.");

                if (!_userContext.CanAccessApartment(apartment))
                    return new ResponseDto(403, message: "No tienes permiso para eliminar esta subzona.");

                // Borrado lógico
                apartment.IsDeleted = true;
                await _context.SaveChangesAsync();

                var message = $"Se marcó como eliminado el departamento '{apartment.Name}' que pertenecía a la zona '{apartment.Zone?.Name}'";

                await _auditLogService.LogManualAsync(
                    action: message,
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: "/Apartment/delete/{id}",
                    httpMethod: "DELETE",
                    statusCode: 200
                );

                return new ResponseDto(200, message: "Departamento eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteApartmentAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar el departamento.");
            }
        }
    }
}