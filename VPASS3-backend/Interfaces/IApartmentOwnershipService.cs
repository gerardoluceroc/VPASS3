using VPASS3_backend.DTOs.ApartmentOwnerships;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces
{
    public interface IApartmentOwnershipService
    {
        Task<ResponseDto> CreateAsync(CreateApartmentOwnershipDto dto);
        Task<ResponseDto> EndCurrentOwnershipAsync(EndApartmentOwnershipDto dto);
        Task<ResponseDto> GetAllAsync();
        Task<ResponseDto> GetByIdAsync(int id);
        Task<ResponseDto> DeleteAsync(int id);
    }
}