using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces.CommonAreaInterfaces
{
    public interface IReservableCommonAreaReservationService
    {
        Task<ResponseDto> CreateAsync(CreateReservableCommonAreaReservationDto dto);
        Task<ResponseDto> GetAllAsync();
        Task<ResponseDto> GetByIdAsync(int id);

        Task<ResponseDto> UpdateAsync(int id, CreateReservableCommonAreaReservationDto dto);
        Task<ResponseDto> DeleteAsync(int id);
    }
}
