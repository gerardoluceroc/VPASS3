using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.AuditLogs;
using VPASS3_backend.Models;

namespace VPASS3_backend.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(HttpContext context, string action, int statusCode);
        Task<ResponseDto> GetAllLogsAsync();
        Task<ResponseDto> GetLogByIdAsync(int id);
        Task<ResponseDto> DeleteLogAsync(int id);
        Task<ResponseDto> UpdateLogAsync(int id, AuditLogDto updatedDto);
    }
}
