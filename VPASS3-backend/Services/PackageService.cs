using VPASS3_backend.Context;
using VPASS3_backend.DTOs.Packages;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Utils;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Models;

namespace VPASS3_backend.Services
{
    public class PackageService : IPackageService
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserContextService _userContext;
        private readonly IAuditLogService _auditLog;

        public PackageService(AppDbContext dbContext, IUserContextService userContext, IAuditLogService auditLog)
        {
            _dbContext = dbContext;
            _userContext = userContext;
            _auditLog = auditLog;
        }

        public async Task<ResponseDto> CreateAsync(CreatePackageDto dto)
        {
            // Se verifica que el departamento exista y tenga zona asociada
            var apartment = await _dbContext.Apartments
                .Include(a => a.Zone)
                .FirstOrDefaultAsync(a => a.Id == dto.IdApartment);

            if (apartment == null)
                return new ResponseDto(404, message: "Departamento no encontrado.");

            if (apartment.Zone == null)
                return new ResponseDto(404, message: "Zona no encontrada para el departamento.");

            // Se verifica permiso por establecimiento si no es SUPERADMIN
            if (_userContext.UserRole != "SUPERADMIN" &&
                (!_userContext.EstablishmentId.HasValue || apartment.Zone.EstablishmentId != _userContext.EstablishmentId.Value))
            {
                return new ResponseDto(403, message: "No tienes permisos para registrar paquetes en este establecimiento.");
            }

            // Se verifica que el registro de propiedad enviado exista
            var ownership = await _dbContext.ApartmentOwnerships
                .Include(o => o.Person)
                .FirstOrDefaultAsync(o => o.Id == dto.IdApartmentOwnership && o.IdApartment == dto.IdApartment);

            if (ownership == null)
                return new ResponseDto(404, message: "Registro de propiedad no válido para este departamento.");

            // Si viene un receptor, se verificar que exista
            Person? receiver = null;
            if (dto.IdPersonWhoReceived.HasValue)
            {
                receiver = await _dbContext.Persons.FindAsync(dto.IdPersonWhoReceived.Value);
                if (receiver == null)
                    return new ResponseDto(404, message: "Persona que retira no encontrada.");
            }

            // Crear paquete
            var package = new Package
            {
                IdApartment = dto.IdApartment,
                IdApartmentOwnership = dto.IdApartmentOwnership,
                Code = dto.Code,
                IdPersonWhoReceived = dto.IdPersonWhoReceived,
                ReceivedAt = TimeHelper.GetSantiagoTime(),
                DeliveredAt = null
            };

            _dbContext.Packages.Add(package);
            await _dbContext.SaveChangesAsync();

            // Se registra en log de auditoría
            await _auditLog.LogManualAsync(
                action: $"Se registró paquete para el dpto {apartment.Name ?? $"ID {apartment.Id}"}, propietario {ownership.Person.Names} {ownership.Person.LastNames}",
                email: _userContext.UserEmail,
                role: _userContext.UserRole,
                userId: _userContext.UserId ?? 0,
                endpoint: "/package/create",
                httpMethod: "POST",
                statusCode: 201
            );
            return new ResponseDto(201, package, "Paquete registrado correctamente.");
        }
    }
}