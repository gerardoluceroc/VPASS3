using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VPASS3_backend.Context;
using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.AuditLogs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;

namespace VPASS3_backend.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public AuditLogService(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        private static DateTime GetSantiagoTime()
        {
            try
            {
                // Linux y contenedores (por ejemplo, Docker en Azure)
                return TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("America/Santiago")
                );
            }
            catch (TimeZoneNotFoundException)
            {
                // Windows Server o desarrollo local en Windows
                return TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time")
                );
            }
        }

        public async Task LogAsync(HttpContext context, string action, int statusCode)
        {
            var user = context.User;

            var email = user.FindFirst(ClaimTypes.Name)?.Value ?? "ANONYMOUS";
            var role = user.FindFirst(ClaimTypes.Role)?.Value ?? "UNASSIGNED";
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int? userId = null;
            if (int.TryParse(userIdClaim, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var method = context.Request.Method;
            var endpoint = context.Request.Path;

            var log = new AuditLog
            {
                Action = action,
                Email = email,
                Role = role,
                UserId = userId,
                HttpMethod = method,
                Endpoint = endpoint,
                StatusCode = statusCode,
                Timestamp = GetSantiagoTime()
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        // Servicio para registrar en los logs el login de un usuario manualmente
        public async Task LogManualAsync(string action, string email, string role, int userId, string endpoint, string httpMethod, int statusCode)
        {
            var log = new AuditLog
            {
                Action = action,
                Email = email,
                Role = role,
                UserId = userId,
                HttpMethod = httpMethod,
                Endpoint = endpoint,
                StatusCode = statusCode,
                Timestamp = GetSantiagoTime()
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }


        public async Task<ResponseDto> GetAllLogsAsync()
        {
            try
            {
                var logs = await _context.AuditLogs.ToListAsync();

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.UserId.HasValue)
                        return new ResponseDto(403, message: "No tienes acceso a estos registros.");

                    logs = logs.Where(log => log.UserId == _userContext.UserId.Value).ToList();
                }

                return new ResponseDto(200, logs, "Logs obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllLogsAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener los logs.");
            }
        }

        public async Task<ResponseDto> GetLogByIdAsync(int id)
        {
            try
            {
                var log = await _context.AuditLogs.FindAsync(id);
                if (log == null)
                    return new ResponseDto(404, message: "Log no encontrado.");

                if (!_userContext.CanAccessOwnResourceById(log.UserId ?? -1))
                    return new ResponseDto(403, message: "No tienes permisos para acceder a este log.");

                return new ResponseDto(200, log, "Log obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetLogByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener el log.");
            }
        }

        public async Task<ResponseDto> DeleteLogAsync(int id)
        {
            try
            {
                var log = await _context.AuditLogs.FindAsync(id);
                if (log == null)
                    return new ResponseDto(404, message: "Log no encontrado.");

                if (!_userContext.CanAccessOwnResourceById(log.UserId ?? -1))
                    return new ResponseDto(403, message: "No tienes permisos para eliminar este log.");

                _context.AuditLogs.Remove(log);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, message: "Log eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteLogAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar el log.");
            }
        }

        public async Task<ResponseDto> UpdateLogAsync(int id, AuditLogDto updatedDto)
        {
            try
            {
                var existingLog = await _context.AuditLogs.FindAsync(id);
                if (existingLog == null)
                    return new ResponseDto(404, message: "Log no encontrado.");

                if (!_userContext.CanAccessOwnResourceById(existingLog.UserId ?? -1))
                    return new ResponseDto(403, message: "No tienes permisos para actualizar este log.");

                // Solo actualiza si el valor no es null o vacío
                existingLog.Action = string.IsNullOrWhiteSpace(updatedDto.Action) ? existingLog.Action : updatedDto.Action;
                existingLog.Email = string.IsNullOrWhiteSpace(updatedDto.Email) ? existingLog.Email : updatedDto.Email;
                existingLog.Role = string.IsNullOrWhiteSpace(updatedDto.Role) ? existingLog.Role : updatedDto.Role;
                existingLog.HttpMethod = string.IsNullOrWhiteSpace(updatedDto.HttpMethod) ? existingLog.HttpMethod : updatedDto.HttpMethod;
                existingLog.Endpoint = string.IsNullOrWhiteSpace(updatedDto.Endpoint) ? existingLog.Endpoint : updatedDto.Endpoint;
                existingLog.StatusCode = updatedDto.StatusCode != 0 ? updatedDto.StatusCode : existingLog.StatusCode;
                existingLog.Timestamp = updatedDto.Timestamp == default ? existingLog.Timestamp : updatedDto.Timestamp;
                existingLog.UserId = updatedDto.UserId ?? existingLog.UserId;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, existingLog, "Log actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateLogAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al actualizar el log.");
            }
        }

    }
}