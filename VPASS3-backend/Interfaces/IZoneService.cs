using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.Zones;

namespace VPASS3_backend.Interfaces
{
    public interface IZoneService
    {
        Task<ResponseDto> GetAllZonesAsync();
        Task<ResponseDto> GetZoneByIdAsync(int id);
        Task<ResponseDto> CreateZoneAsync(CreateZoneDto dto);
        Task<ResponseDto> UpdateZoneAsync(int id, CreateZoneDto dto);
        Task<ResponseDto> DeleteZoneAsync(int id);
    }
}
