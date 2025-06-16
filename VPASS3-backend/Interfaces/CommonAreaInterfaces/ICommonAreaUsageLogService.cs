using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces.CommonAreaInterfaces
{
    public interface ICommonAreaUsageLogService
    {
        Task<ResponseDto> GetAllAsync();
        Task<ResponseDto> GetByIdAsync(int id);
        Task<ResponseDto> CreateUsageAsync(CreateUsageLogDto dto);
        Task<ResponseDto> UpdateAsync(int id, UpdateUsageLogDto dto);
        Task<ResponseDto> DeleteUsageAsync(int id);
    }
}