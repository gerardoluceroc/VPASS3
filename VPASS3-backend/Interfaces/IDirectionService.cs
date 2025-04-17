using VPASS3_backend.DTOs.Directions;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces
{
    public interface IDirectionService
    {
        Task<ResponseDto> GetAllDirectionsAsync();
        Task<ResponseDto> GetDirectionByIdAsync(int id);
        Task<ResponseDto> CreateDirectionAsync(DirectionDto dto);
        Task<ResponseDto> UpdateDirectionAsync(int id, DirectionDto dto);
        Task<ResponseDto> DeleteDirectionAsync(int id);
    }
}

