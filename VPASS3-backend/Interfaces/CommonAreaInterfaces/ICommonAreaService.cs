using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces.CommonAreaInterfaces
{
    public interface ICommonAreaService
    {
        Task<ResponseDto> GetAllAsync();
        Task<ResponseDto> GetByIdAsync(int id);
        Task<ResponseDto> CreateAsync(CreateCommonAreaDto dto);
        Task<ResponseDto> UpdateAsync(int id, UpdateCommonAreaDto dto);
        Task<ResponseDto> DeleteAsync(int id);
    }
}
