using VPASS3_backend.Context;
using VPASS3_backend.DTOs.Zones;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace VPASS3_backend.Services
{
    public class ZoneService : IZoneService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;
        private readonly IAuditLogService _auditLogService;

        public ZoneService(AppDbContext context, IUserContextService userContext, IAuditLogService auditLogService)
        {
            _context = context;
            _userContext = userContext;
            _auditLogService = auditLogService;
        }

        public async Task<ResponseDto> GetAllZonesAsync()
        {
            try
            {
                // Obtener zonas no eliminadas junto con sus subzonas
                var zones = await _context.Zones
                    .Where(z => !z.IsDeleted)
                    .Include(z => z.Establishment)
                    .Include(z => z.ZoneSections)
                    .ToListAsync();

                // Filtrar por establecimiento si no es SUPERADMIN
                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    zones = zones
                        .Where(z => _userContext.CanAccessZone(z))
                        .ToList();
                }

                // Filtrar las subzonas eliminadas manualmente
                foreach (var zone in zones)
                {
                    zone.ZoneSections = zone.ZoneSections
                        .Where(zs => !zs.IsDeleted)
                        .ToList();
                }

                return new ResponseDto(200, zones, message: "Zonas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetAllZonesAsync: {ex.Message}");
                return new ResponseDto(500, message: "Error en el servidor al obtener las zonas.");
            }
        }


        public async Task<ResponseDto> GetZoneByIdAsync(int id)
        {
            try
            {
                var zone = await _context.Zones
                    .Include(z => z.Establishment)
                    .Include(z => z.ZoneSections)
                    .FirstOrDefaultAsync(z => z.Id == id && !z.IsDeleted);

                if (zone == null)
                    return new ResponseDto(404, message: "Zona no encontrada.");

                if (!_userContext.CanAccessZone(zone))
                    return new ResponseDto(403, message: "No tienes permisos para acceder a esta zona.");

                // Filtrar subzonas eliminadas
                zone.ZoneSections = zone.ZoneSections
                    .Where(zs => !zs.IsDeleted)
                    .ToList();

                return new ResponseDto(200, zone, message: "Zona obtenida correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetZoneByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener la zona.");
            }
        }


        public async Task<ResponseDto> CreateZoneAsync(CreateZoneDto dto)
        {
            try
            {
                var establishment = await _context.Establishments
                    .FirstOrDefaultAsync(e => e.Id == dto.EstablishmentId);

                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                // Verificar duplicado (solo zonas activas)
                var exists = await _context.Zones.AnyAsync(z =>
                    !z.IsDeleted &&
                    z.Name == dto.Name &&
                    z.EstablishmentId == dto.EstablishmentId);

                if (exists)
                    return new ResponseDto(409, message: "Ya existe una zona con ese nombre en este establecimiento.");

                var zone = new Zone
                {
                    Name = dto.Name,
                    EstablishmentId = dto.EstablishmentId,
                    IsDeleted = false
                };

                if (!_userContext.CanAccessZone(zone))
                    return new ResponseDto(403, message: "No tienes permisos para acceder a esta zona.");

                _context.Zones.Add(zone);
                await _context.SaveChangesAsync();

                var message = $"Se creó la zona '{zone.Name}'";

                await _auditLogService.LogManualAsync(
                    action: message,
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: "/Zone/create",
                    httpMethod: "POST",
                    statusCode: 201
                );

                return new ResponseDto(201, zone, message: "Zona creada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateZoneAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al crear la zona.");
            }
        }

        public async Task<ResponseDto> UpdateZoneAsync(int id, CreateZoneDto dto)
        {
            try
            {
                var zone = await _context.Zones
                    .Include(z => z.Establishment)
                    .FirstOrDefaultAsync(z => z.Id == id && !z.IsDeleted);

                if (zone == null)
                    return new ResponseDto(404, message: "Zona no encontrada.");

                var establishment = await _context.Establishments
                    .FirstOrDefaultAsync(e => e.Id == dto.EstablishmentId);

                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                // Verificar nombre duplicado
                var nameExists = await _context.Zones.AnyAsync(z =>
                    !z.IsDeleted &&
                    z.Id != id &&
                    z.Name == dto.Name &&
                    z.EstablishmentId == dto.EstablishmentId);

                if (nameExists)
                    return new ResponseDto(409, message: "Ya existe una zona con ese nombre en este establecimiento.");

                if (!_userContext.CanAccessZone(zone))
                    return new ResponseDto(403, message: "No tienes permisos para acceder a esta zona.");

                zone.Name = dto.Name;
                zone.EstablishmentId = dto.EstablishmentId;

                await _context.SaveChangesAsync();

                var message = $"Se actualizó la zona '{zone.Name}'";

                await _auditLogService.LogManualAsync(
                    action: message,
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: $"/Zone/update/{id}",
                    httpMethod: "PUT",
                    statusCode: 200
                );

                return new ResponseDto(200, zone, message: "Zona actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateZoneAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al actualizar la zona.");
            }
        }

        public async Task<ResponseDto> DeleteZoneAsync(int id)
        {
            try
            {
                var zone = await _context.Zones
                    .Include(z => z.ZoneSections)
                    .FirstOrDefaultAsync(z => z.Id == id);

                if (zone == null || zone.IsDeleted)
                    return new ResponseDto(404, message: "Zona no encontrada o ya eliminada.");

                if (!_userContext.CanAccessZone(zone))
                    return new ResponseDto(403, message: "No tienes permisos para eliminar esta zona.");

                // Marcar zona como eliminada
                zone.IsDeleted = true;

                // Marcar todas sus subzonas como eliminadas
                foreach (var section in zone.ZoneSections)
                {
                    section.IsDeleted = true;
                }

                await _context.SaveChangesAsync();

                var message = $"Se marcó como eliminada la zona '{zone.Name}' y todas sus subzonas asociadas.";

                await _auditLogService.LogManualAsync(
                    action: message,
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: $"/Zone/delete/{id}",
                    httpMethod: "DELETE",
                    statusCode: 200
                );

                return new ResponseDto(200, message: "Zona y subzonas eliminadas lógicamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteZoneAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar la zona.");
            }
        }


    }
}