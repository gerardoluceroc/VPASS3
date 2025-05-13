using VPASS3_backend.DTOs.ParkingSpots;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces
{
    public interface IParkingSpotService
    {
        Task<ResponseDto> GetAllParkingSpotsAsync();
        Task<ResponseDto> GetParkingSpotByIdAsync(int id);
        Task<ResponseDto> CreateParkingSpotAsync(ParkingSpotDto dto);
        Task<ResponseDto> UpdateParkingSpotAsync(int id, UpdateParkingSpotDto dto);
        Task<ResponseDto> DeleteParkingSpotAsync(int id);
    }
}

