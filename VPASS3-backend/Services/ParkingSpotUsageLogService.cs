using VPASS3_backend.Context;
using VPASS3_backend.DTOs.ParkingSpotUsageLogs;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace VPASS3_backend.Services
{
    public class ParkingSpotUsageLogService : IParkingSpotUsageLogService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public ParkingSpotUsageLogService(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<ResponseDto> CreateAsync(ParkingSpotUsageLogDto dto)
        {
            try
            {
                var visit = await _context.Visits
                    .Include(v => v.Direction)
                    .FirstOrDefaultAsync(v => v.Id == dto.IdVisit);
                var parkingSpot = await _context.ParkingSpots.FindAsync(visit.IdParkingSpot);
                var visitor = await _context.Persons.FindAsync(visit.IdPerson);

                if (parkingSpot == null)
                    return new ResponseDto(404, message: "Estacionamiento no encontrado.");
                if (visitor == null)
                    return new ResponseDto(404, message: "Visitante no encontrado.");
                if (visit == null)
                    return new ResponseDto(404, message: "Visita no encontrada.");

                // Validar acceso a establecimiento
                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != parkingSpot.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permisos para registrar uso en este estacionamiento.");
                }

                // Visita tipo ENTRADA
                if (visit.Direction.VisitDirection.ToLower() == "entrada" && visit.VehicleIncluded)
                {
                    var newLog = new ParkingSpotUsageLog
                    {
                        IdEntryVisit = visit.Id,
                        StartTime = visit.EntryDate,
                        AuthorizedTime = visit.AuthorizedTime,
                    };

                    _context.ParkingSpotUsageLogs.Add(newLog);
                    await _context.SaveChangesAsync();

                    return new ResponseDto(201, newLog, "Registro de entrada al estacionamiento creado.");
                }

                // Visita tipo SALIDA que incluye vehiculo
                if (visit.Direction.VisitDirection.ToLower() == "salida" && visit.VehicleIncluded)
                {
                    var openLog = await _context.ParkingSpotUsageLogs
                        .Where(p => p.EntryVisit.Person.Id == visit.IdPerson && p.IdExitVisit == null && p.EndTime == null)
                        .OrderByDescending(p => p.StartTime)
                        .FirstOrDefaultAsync();

                    if (openLog == null)
                        return new ResponseDto(404, message: "No se encontró un uso de estacionamiento abierto para este visitante.");

                    openLog.IdExitVisit = visit.Id;
                    openLog.EndTime = visit.EntryDate;
                    openLog.UsageTime = openLog.EndTime.Value - openLog.StartTime;

                    await _context.SaveChangesAsync();

                    return new ResponseDto(200, openLog, "Registro de salida actualizado correctamente.");
                }

                return new ResponseDto(400, message: "Dirección de la visita inválida. Debe ser entrada o salida.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateAsync ParkingSpotUsageLogService: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al registrar el uso del estacionamiento.");
            }
        }

        public async Task<ResponseDto> GetAllAsync()
        {
            try
            {
                var logs = await _context.ParkingSpotUsageLogs
                .Include(p => p.EntryVisit)
                    .ThenInclude(v => v.Zone)
                .Include(p => p.EntryVisit)
                    .ThenInclude(v => v.ZoneSection)
                .Include(p => p.EntryVisit)
                    .ThenInclude(v => v.Direction)
                .Include(p => p.EntryVisit)
                    .ThenInclude(v => v.VisitType)
                .Include(p => p.EntryVisit)
                    .ThenInclude(v => v.ParkingSpot)
                .Include(p => p.EntryVisit)
                    .ThenInclude(v => v.Person)
                .ToListAsync();

                // Si el usuario no es SUPERADMIN, se filtra por su establecimiento
                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    logs = logs
                        .Where(p => p.EntryVisit.EstablishmentId == _userContext.EstablishmentId)
                        .ToList();
                }

                return new ResponseDto(200, logs, "Registros de uso de estacionamiento obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllAsync ParkingSpotUsageLogService: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener los registros.");
            }
        }

        public async Task<ResponseDto> GetByIdAsync(int id)
        {
            try
            {
                var log = await _context.ParkingSpotUsageLogs
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (log == null)
                    return new ResponseDto(404, message: "Registro no encontrado.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    log.EntryVisit.ParkingSpot.Id != _userContext.EstablishmentId)
                {
                    return new ResponseDto(403, message: "No tienes permisos para acceder a este registro.");
                }

                return new ResponseDto(200, log, "Registro obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetByIdAsync ParkingSpotUsageLogService: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener el registro.");
            }
        }

        public async Task<ResponseDto> DeleteAsync(int id)
        {
            try
            {
                var log = await _context.ParkingSpotUsageLogs
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (log == null)
                    return new ResponseDto(404, message: "Registro no encontrado.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    log.EntryVisit.ParkingSpot.Id != _userContext.EstablishmentId)
                {
                    return new ResponseDto(403, message: "No tienes permisos para eliminar este registro.");
                }

                _context.ParkingSpotUsageLogs.Remove(log);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, message: "Registro eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteAsync ParkingSpotUsageLogService: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar el registro.");
            }
        }
    }
}