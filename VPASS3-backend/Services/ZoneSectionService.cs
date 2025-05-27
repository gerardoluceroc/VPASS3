using VPASS3_backend.Context;
using VPASS3_backend.DTOs.ZoneSections;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace VPASS3_backend.Services
{
    public class ZoneSectionService : IZoneSectionService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;
        private readonly IAuditLogService _auditLogService;

        public ZoneSectionService(AppDbContext context, IUserContextService userContext, IAuditLogService auditLogService)
        {
            _context = context;
            _userContext = userContext;
            _auditLogService = auditLogService;
        }

        public async Task<ResponseDto> GetAllZoneSectionsAsync()
        {
            try
            {
                var sections = await _context.ZoneSections
                    .Where(zs => !zs.IsDeleted)
                    .Include(zs => zs.Zone)
                    .ToListAsync();

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    sections = sections
                        .Where(zs => _userContext.CanAccessZoneSection(zs))
                        .ToList();
                }

                return new ResponseDto(200, sections, "Subzonas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllZoneSectionsAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener las Subzonas.");
            }
        }

        public async Task<ResponseDto> GetZoneSectionByIdAsync(int id)
        {
            try
            {
                var section = await _context.ZoneSections
                    .Include(zs => zs.Zone)
                    .FirstOrDefaultAsync(zs => zs.Id == id && !zs.IsDeleted);

                if (section == null)
                    return new ResponseDto(404, message: "Subzona no encontrada.");

                if (!_userContext.CanAccessZoneSection(section))
                    return new ResponseDto(403, message: "No tienes permiso para acceder a esta subzona.");

                return new ResponseDto(200, section, "Subzona obtenida correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetZoneSectionByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener la subzona.");
            }
        }

        public async Task<ResponseDto> CreateZoneSectionAsync(ZoneSectionDto dto)
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
                    return new ResponseDto(403, message: "No tienes permiso para crear sub zonas en esta zona.");
                }

                //  Validar si ya existe una subzona con el mismo nombre (no eliminada)
                var exists = await _context.ZoneSections
                    .AnyAsync(zs => zs.Name == dto.Name && zs.IdZone == dto.IdZone && !zs.IsDeleted);

                if (exists)
                    return new ResponseDto(409, message: "Ya existe una subzona con ese nombre en esta zona.");

                var zoneSection = new ZoneSection
                {
                    Name = dto.Name,
                    IdZone = dto.IdZone
                };

                _context.ZoneSections.Add(zoneSection);
                await _context.SaveChangesAsync();

                var message = $"Se creó la Subzona '{zoneSection.Name}' que es parte de la zona '{zone.Name}'";

                await _auditLogService.LogManualAsync(
                    action: message,
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: "/ZoneSection/create",
                    httpMethod: "POST",
                    statusCode: 201
                );

                return new ResponseDto(201, zoneSection, message: "Sección creada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateZoneSectionAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al crear la sección.");
            }
        }

        public async Task<ResponseDto> UpdateZoneSectionAsync(int id, ZoneSectionDto dto)
        {
            try
            {
                var section = await _context.ZoneSections
                    .Include(zs => zs.Zone)
                    .FirstOrDefaultAsync(zs => zs.Id == id);

                if (section == null)
                    return new ResponseDto(404, message: "Subzona no encontrada.");

                if (!_userContext.CanAccessZoneSection(section))
                    return new ResponseDto(403, message: "No tienes permiso para editar esta subzona.");

                var zone = await _context.Zones.FirstOrDefaultAsync(z => z.Id == dto.IdZone);
                if (zone == null)
                    return new ResponseDto(404, message: "Zona asociada no encontrada.");

                //  Validación de duplicado (si cambia de nombre o zona)
                bool isDuplicate = await _context.ZoneSections.AnyAsync(zs =>
                    zs.Id != id &&
                    zs.IdZone == dto.IdZone &&
                    zs.Name == dto.Name &&
                    !zs.IsDeleted);

                if (isDuplicate)
                    return new ResponseDto(409, message: "Ya existe una subzona con ese nombre en esta zona.");

                // Actualizar campos
                section.Name = dto.Name;
                section.IdZone = dto.IdZone;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, section, "Subzona actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateZoneSectionAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al actualizar la subzona.");
            }
        }

        public async Task<ResponseDto> DeleteZoneSectionAsync(int id)
        {
            try
            {
                var section = await _context.ZoneSections
                    .Include(zs => zs.Zone)
                    .FirstOrDefaultAsync(zs => zs.Id == id && !zs.IsDeleted);

                if (section == null)
                    return new ResponseDto(404, message: "Subzona no encontrada.");

                if (!_userContext.CanAccessZoneSection(section))
                    return new ResponseDto(403, message: "No tienes permiso para eliminar esta subzona.");

                // Borrado lógico
                section.IsDeleted = true;
                await _context.SaveChangesAsync();

                var message = $"Se marcó como eliminada la Subzona '{section.Name}' que pertenecía a la zona '{section.Zone?.Name}'";

                await _auditLogService.LogManualAsync(
                    action: message,
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: "/ZoneSection/delete/{id}",
                    httpMethod: "DELETE",
                    statusCode: 200
                );

                return new ResponseDto(200, message: "Subzona eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteZoneSectionAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar la subzona.");
            }
        }
    }
}
