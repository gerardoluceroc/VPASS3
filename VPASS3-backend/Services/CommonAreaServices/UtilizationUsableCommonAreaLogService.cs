using VPASS3_backend.Context;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces.CommonAreaInterfaces;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models.CommonAreas.UsableCommonArea;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.Utils;

namespace VPASS3_backend.Services.CommonAreaServices
{
    public class UtilizationUsableCommonAreaLogService : IUtilizationUsableCommonAreaLogService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public UtilizationUsableCommonAreaLogService(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<ResponseDto> CreateAsync(CreateUtilizationUsableCommonAreaLogDto dto)
        {
            try
            {
                var area = await _context.UsableCommonAreas
                    .Include(a => a.Establishment)
                    .FirstOrDefaultAsync(a => a.Id == dto.IdUsableCommonArea);

                if (area == null)
                    return new ResponseDto(404, message: "Área común utilizable no encontrada.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != area.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permisos para registrar uso en esta área.");
                }

                var person = await _context.Persons.FindAsync(dto.IdPerson);
                if (person == null)
                    return new ResponseDto(404, message: "Persona no encontrada.");

                var log = new UtilizationUsableCommonAreaLog
                {
                    IdUsableCommonArea = dto.IdUsableCommonArea,
                    IdPerson = dto.IdPerson,
                    StartTime = TimeHelper.GetSantiagoTime(),
                    UsageTime = dto.UsageTime,
                    GuestsNumber = dto.GuestsNumber,
                };

                _context.UtilizationUsableCommonAreaLogs.Add(log);
                await _context.SaveChangesAsync();

                return new ResponseDto(201, log, "Registro de uso creado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateAsync UtilizationUsableCommonAreaLog: " + ex.Message);
                return new ResponseDto(500, message: "Error al crear el registro de uso.");
            }
        }

        public async Task<ResponseDto> GetAllAsync()
        {
            try
            {
                var logs = await _context.UtilizationUsableCommonAreaLogs
                    .Include(l => l.UsableCommonArea)
                    .Include(l => l.Person)
                    .ToListAsync();

                // Filtrar por establecimiento si no es SUPERADMIN
                if (_userContext.UserRole != "SUPERADMIN" && _userContext.EstablishmentId.HasValue)
                {
                    logs = logs
                        .Where(l => l.UsableCommonArea.IdEstablishment == _userContext.EstablishmentId)
                        .ToList();
                }

                return new ResponseDto(200, logs, "Registros de uso obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllAsync UtilizationLog: " + ex.Message);
                return new ResponseDto(500, message: "Error al obtener los registros.");
            }
        }

        public async Task<ResponseDto> GetByIdAsync(int id)
        {
            try
            {
                var log = await _context.UtilizationUsableCommonAreaLogs
                    .Include(l => l.UsableCommonArea)
                    .Include(l => l.Person)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (log == null)
                    return new ResponseDto(404, message: "Registro de uso no encontrado.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != log.UsableCommonArea.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permisos para acceder a este registro.");
                }

                return new ResponseDto(200, log, "Registro de uso obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetByIdAsync UtilizationLog: " + ex.Message);
                return new ResponseDto(500, message: "Error al obtener el registro.");
            }
        }

        public async Task<ResponseDto> UpdateAsync(int id, CreateUtilizationUsableCommonAreaLogDto dto)
        {
            try
            {
                var log = await _context.UtilizationUsableCommonAreaLogs
                    .Include(l => l.UsableCommonArea)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (log == null)
                    return new ResponseDto(404, message: "Registro no encontrado.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != log.UsableCommonArea.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permisos para modificar este registro.");
                }

                log.UsageTime = dto.UsageTime;

                log.GuestsNumber = dto.GuestsNumber;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, log, "Registro de uso actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateAsync UtilizationLog: " + ex.Message);
                return new ResponseDto(500, message: "Error al actualizar el registro.");
            }
        }


        public async Task<ResponseDto> DeleteAsync(int id)
        {
            try
            {
                var log = await _context.UtilizationUsableCommonAreaLogs
                    .Include(l => l.UsableCommonArea)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (log == null)
                    return new ResponseDto(404, message: "Registro no encontrado.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != log.UsableCommonArea.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permisos para eliminar este registro.");
                }

                _context.UtilizationUsableCommonAreaLogs.Remove(log);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, message: "Registro eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteAsync UtilizationLog: " + ex.Message);
                return new ResponseDto(500, message: "Error al eliminar el registro.");
            }
        }
    }
}