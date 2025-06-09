using VPASS3_backend.DTOs.Persons;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces
{
    public interface IPersonService
    {
        Task<ResponseDto> GetAllAsync();
        Task<ResponseDto> GetByIdAsync(int id);
        Task<ResponseDto> CreateAsync(CreatePersonDto dto);
        Task<ResponseDto> UpdateAsync(int id, CreatePersonDto dto);
        Task<ResponseDto> DeleteAsync(int id);
    }
}