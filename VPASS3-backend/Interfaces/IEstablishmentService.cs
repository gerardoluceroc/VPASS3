using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.Establishments;
using VPASS3_backend.Models;

namespace VPASS3_backend.Interfaces
{
    public interface IEstablishmentService
    {
        Task<ResponseDto> CreateEstablishmentAsync(CreateEstablishmentDto createEstablishmentDto);
        Task<ResponseDto> GetAllEstablishmentsAsync();
        Task<ResponseDto> GetEstablishmentByIdAsync(int id);
        Task<ResponseDto> UpdateEstablishmentAsync(int id, CreateEstablishmentDto createEstablishmentDto);
        Task<ResponseDto> DeleteEstablishmentAsync(int id);
    }



}
