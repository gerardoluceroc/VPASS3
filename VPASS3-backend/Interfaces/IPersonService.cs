using VPASS3_backend.DTOs.Persons;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces
{
    public interface IPersonService
    {
        Task<ResponseDto> GetAllPersonsAsync();
        Task<ResponseDto> GetPersonByIdAsync(int id);
        Task<ResponseDto> GetPersonByIdentificationNumberAsync(string identificationNumber);
        Task<ResponseDto> CreatePersonAsync(PersonDto dto);
        Task<ResponseDto> UpdatePersonAsync(int id, PersonDto dto);
        Task<ResponseDto> DeletePersonAsync(int id);
    }
}
