using VPASS3_backend.Context;
using VPASS3_backend.DTOs.Visits;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace VPASS3_backend.Services
{
    public class VisitService : IVisitService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public VisitService(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }


        public async Task<ResponseDto> GetAllVisitsAsync()
        {
            try
            {
                var visits = await _context.Visits
                    .Include(v => v.Establishment)
                    .ToListAsync();

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message:"No tienes un establecimiento asociado.");

                    visits = visits
                        .Where(v => _userContext.CanAccessVisit(v))
                        .ToList();
                }

                return new ResponseDto(200, visits, "Visitas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllVisitsAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener las visitas.");
            }
        }


        public async Task<ResponseDto> GetVisitByIdAsync(int id)
        {
            try
            {
                var visit = await _context.Visits
                    .Include(v => v.Establishment)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (visit == null)
                    return new ResponseDto(404, "Visita no encontrada.");

                if (!_userContext.CanAccessVisit(visit))
                    return new ResponseDto(403, message:"No tienes permiso para acceder a esta visita.");

                return new ResponseDto(200, visit, "Visita obtenida correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetVisitByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener la visita.");
            }
        }


        public async Task<ResponseDto> CreateVisitAsync(VisitDto dto)
        {
            try
            {
                // Verificar permisos sobre el establecimiento
                if (!_userContext.CanAccessOwnEstablishment(dto.EstablishmentId))
                {
                    return new ResponseDto(403, message: "No tienes permiso para crear visitas en este establecimiento.");
                }

                // Validar existencia del establecimiento
                var establishmentExists = await _context.Establishments.AnyAsync(e => e.Id == dto.EstablishmentId);
                if (!establishmentExists)
                {
                    return new ResponseDto(404, message: "El establecimiento especificado no existe.");
                }

                // Validar existencia del tipo de visita
                var visitType = await _context.VisitTypes.FirstOrDefaultAsync(vt => vt.Id == dto.IdVisitType);
                if (visitType == null)
                {
                    return new ResponseDto(404, message: "El tipo de visita especificado no existe.");
                }

                if (!_userContext.CanAccessVisitType(visitType) || visitType.IdEstablishment != dto.EstablishmentId)
                {
                    return new ResponseDto(403, message: "El tipo de visita no pertenece a tu establecimiento.");
                }

                // Validar existencia de la zona
                var zone = await _context.Zones.FirstOrDefaultAsync(z => z.Id == dto.ZoneId);
                if (zone == null)
                {
                    return new ResponseDto(404, message: "La zona especificada no existe.");
                }

                if (!_userContext.CanAccessZone(zone) || zone.EstablishmentId != dto.EstablishmentId)
                {
                    return new ResponseDto(403, message: "La zona no pertenece a tu establecimiento.");
                }

                // Validar existencia de la zona sección (si aplica)
                int? idZoneSectionValida = null;
                if (dto.IdZoneSection.HasValue)
                {
                    var zoneSection = await _context.ZoneSections
                        .Include(zs => zs.Zone)
                        .FirstOrDefaultAsync(zs => zs.Id == dto.IdZoneSection.Value && zs.IdZone == dto.ZoneId);

                    if (zoneSection == null)
                    {
                        return new ResponseDto(404, message: "La subzona especificada no existe o no está asociada a la zona indicada.");
                    }

                    if (!_userContext.CanAccessZoneSection(zoneSection))
                    {
                        return new ResponseDto(403, message: "No tienes acceso a la subzona especificada.");
                    }

                    idZoneSectionValida = zoneSection.Id;
                }

                // Obtener hora local Chile
                TimeZoneInfo chileTimeZone;
                try
                {
                    chileTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time");
                }
                catch
                {
                    chileTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Santiago");
                }

                DateTime chileDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, chileTimeZone);

                // Crear la visita
                var visit = new Visit
                {
                    EstablishmentId = dto.EstablishmentId,
                    VisitorId = dto.VisitorId,
                    ZoneId = dto.ZoneId,
                    IdDirection = dto.IdDirection,
                    VehicleIncluded = dto.VehicleIncluded,
                    LicensePlate = dto.VehicleIncluded ? dto.LicensePlate : null,
                    IdParkingSpot = dto.VehicleIncluded ? dto.IdParkingSpot : null,
                    EntryDate = chileDateTime,
                    IdZoneSection = idZoneSectionValida,
                    IdVisitType = dto.IdVisitType,
                };

                _context.Visits.Add(visit);
                await _context.SaveChangesAsync();

                return new ResponseDto(201, visit, "Visita creada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateVisitAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al crear la visita.");
            }
        }

        public async Task<ResponseDto> UpdateVisitAsync(int id, VisitDto dto)
        {
            try
            {
                var visit = await _context.Visits
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (visit == null)
                    return new ResponseDto(404, message: "Visita no encontrada.");

                if (!_userContext.CanAccessVisit(visit))
                    return new ResponseDto(403, message: "No tienes permiso para editar esta visita.");

                if (!_userContext.CanAccessOwnEstablishment(dto.EstablishmentId))
                    return new ResponseDto(403, message: "No puedes reasignar la visita a otro establecimiento.");

                // Verificar existencia del nuevo establecimiento
                var establishmentExists = await _context.Establishments.AnyAsync(e => e.Id == dto.EstablishmentId);
                if (!establishmentExists)
                {
                    return new ResponseDto(404, message: "El establecimiento especificado no existe.");
                }

                // Verificar existencia y pertenencia del tipo de visita
                var visitType = await _context.VisitTypes.FirstOrDefaultAsync(vt => vt.Id == dto.IdVisitType);
                if (visitType == null)
                {
                    return new ResponseDto(404, message: "El tipo de visita especificado no existe.");
                }

                if (!_userContext.CanAccessVisitType(visitType) || visitType.IdEstablishment != dto.EstablishmentId)
                {
                    return new ResponseDto(403, message: "El tipo de visita no pertenece a tu establecimiento.");
                }

                // Verificar existencia y pertenencia de la zona
                var zone = await _context.Zones.FirstOrDefaultAsync(z => z.Id == dto.ZoneId);
                if (zone == null)
                {
                    return new ResponseDto(404, message: "La zona especificada no existe.");
                }

                if (!_userContext.CanAccessZone(zone) || zone.EstablishmentId != dto.EstablishmentId)
                {
                    return new ResponseDto(403, message: "La zona no pertenece a tu establecimiento.");
                }

                // Verificar existencia y pertenencia de la zona sección (opcional)
                int? idZoneSectionValida = null;
                if (dto.IdZoneSection.HasValue)
                {
                    var zoneSection = await _context.ZoneSections
                        .Include(zs => zs.Zone)
                        .FirstOrDefaultAsync(zs => zs.Id == dto.IdZoneSection.Value && zs.IdZone == dto.ZoneId);

                    if (zoneSection == null)
                    {
                        return new ResponseDto(404, message: "La subzona especificada no existe o no está asociada a la zona indicada.");
                    }

                    if (!_userContext.CanAccessZoneSection(zoneSection))
                    {
                        return new ResponseDto(403, message: "No tienes acceso a la subzona especificada.");
                    }

                    idZoneSectionValida = zoneSection.Id;
                }

                // Asignar los nuevos valores
                visit.EstablishmentId = dto.EstablishmentId;
                visit.VisitorId = dto.VisitorId;
                visit.ZoneId = dto.ZoneId;
                visit.VehicleIncluded = dto.VehicleIncluded;
                visit.IdDirection = dto.IdDirection;
                visit.IdZoneSection = idZoneSectionValida;
                visit.LicensePlate = dto.VehicleIncluded ? dto.LicensePlate : null;
                visit.IdParkingSpot = dto.VehicleIncluded ? dto.IdParkingSpot : null;
                visit.IdVisitType = dto.IdVisitType;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, visit, "Visita actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateVisitAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al actualizar la visita.");
            }
        }

        public async Task<ResponseDto> DeleteVisitAsync(int id)
        {
            try
            {
                var visit = await _context.Visits
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (visit == null)
                    return new ResponseDto(404, message: "Visita no encontrada.");

                if (!_userContext.CanAccessVisit(visit))
                    return new ResponseDto(403, message: "No tienes permiso para eliminar esta visita.");

                _context.Visits.Remove(visit);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, message: "Visita eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteVisitAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar la visita.");
            }
        }

    }
}
