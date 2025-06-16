using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces.CommonAreaInterfaces
{
    public interface ICommonAreaReservationService
    {
        Task<ResponseDto> GetAllAsync();
        Task<ResponseDto> GetByIdAsync(int id);
        Task<ResponseDto> CreateAsync(CreateCommonAreaReservationDto dto);
        Task<ResponseDto> UpdateAsync(int id, UpdateCommonAreaReservationDto dto);
        Task<ResponseDto> DeleteAsync(int id);
    }
}
