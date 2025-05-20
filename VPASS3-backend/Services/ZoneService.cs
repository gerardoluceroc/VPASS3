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
                // Se traen todas las zonas desde la base de datos
                var zones = await _context.Zones
                    .Include(z => z.Establishment)
                    .Include(z => z.ZoneSections)
                    .ToListAsync();

                // 2. Si el usuario NO es SUPERADMIN, se aplica el filtro en memoria
                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    zones = zones
                        .Where(z => _userContext.CanAccessZone(z))
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
                    .FirstOrDefaultAsync(z => z.Id == id);

                if (zone == null)
                    return new ResponseDto(404, message: "Zona no encontrada.");

                if (!_userContext.CanAccessZone(zone))
                    return new ResponseDto(403, message: "No tienes permisos para acceder a esta zona.");

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
                // Verificar si el Establishment existe
                var establishment = await _context.Establishments
                    .FirstOrDefaultAsync(e => e.Id == dto.EstablishmentId);

                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                // Crear la nueva zona
                var zone = new Zone
                {
                    Name = dto.Name,
                    EstablishmentId = dto.EstablishmentId
                };

                if (!_userContext.CanAccessZone(zone))
                    return new ResponseDto(403, message: "No tienes permisos para acceder a esta zona.");

                // Guardar la zona en la base de datos
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
                    statusCode: 200
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
                    .FirstOrDefaultAsync(z => z.Id == id);

                if (zone == null)
                    return new ResponseDto(404, message: "Zona no encontrada.");

                // Verificar si el Establishment existe
                var establishment = await _context.Establishments
                    .FirstOrDefaultAsync(e => e.Id == dto.EstablishmentId);

                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                // Actualizar los datos de la zona
                zone.Name = dto.Name;
                zone.EstablishmentId = dto.EstablishmentId;

                if (!_userContext.CanAccessZone(zone))
                    return new ResponseDto(403, message: "No tienes permisos para acceder a esta zona.");

                await _context.SaveChangesAsync();

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
                var zone = await _context.Zones.FindAsync(id);

                if (zone == null)
                    return new ResponseDto(404, message: "Zona no encontrada.");

                if (!_userContext.CanAccessZone(zone))
                    return new ResponseDto(403, message: "No tienes permisos para acceder a esta zona.");

                _context.Zones.Remove(zone);
                await _context.SaveChangesAsync();

                var message = $"Se eliminó la zona '{zone.Name}'";

                await _auditLogService.LogManualAsync(
                    action: message,
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: "/Zone/delete/{id}",
                    httpMethod: "DELETE",
                    statusCode: 200
                );

                return new ResponseDto(200, message:"Zona eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteZoneAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar la zona.");
            }
        }
    }
}