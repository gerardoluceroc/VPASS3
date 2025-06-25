using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.Apartments;

namespace VPASS3_backend.Interfaces
{
    public interface IApartmentService
    {
        Task<ResponseDto> GetAllApartmentsAsync();
        Task<ResponseDto> GetApartmentByIdAsync(int id);
        Task<ResponseDto> CreateApartmentAsync(ApartmentDto dto);
        Task<ResponseDto> UpdateApartmentAsync(int id, ApartmentDto dto);
        Task<ResponseDto> DeleteApartmentAsync(int id);
    }
}