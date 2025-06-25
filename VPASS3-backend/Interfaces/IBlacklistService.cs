using VPASS3_backend.DTOs.Blacklist;
using VPASS3_backend.DTOs;


namespace VPASS3_backend.Interfaces
{
    public interface IBlacklistService
    {
        Task<ResponseDto> CreateAsync(BlacklistDto dto);
        Task<ResponseDto> DeleteByPersonAsync(DeleteBlacklistByPersonIdDto dto);
        Task<ResponseDto> GetAllAsync();
        Task<ResponseDto> UpdateAsync(int id, BlacklistDto dto);
        Task<ResponseDto> GetByIdAsync(int id);
        Task<ResponseDto> DeleteAsync(int id);
    }
}