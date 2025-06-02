using VPASS3_backend.DTOs.ParkingSpotUsageLogs;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces
{
    public interface IParkingSpotUsageLogService
    {
        Task<ResponseDto> CreateAsync(ParkingSpotUsageLogDto dto);
        Task<ResponseDto> GetAllAsync();
        Task<ResponseDto> GetByIdAsync(int id);
        Task<ResponseDto> DeleteAsync(int id);

    }
}
