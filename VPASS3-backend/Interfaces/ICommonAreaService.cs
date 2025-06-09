using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.CommonAreas;

namespace VPASS3_backend.Interfaces
{
    public interface ICommonAreaService
    {
        Task<ResponseDto> GetAllCommonAreasAsync();
        Task<ResponseDto> GetCommonAreaByIdAsync(int id);
        Task<ResponseDto> CreateCommonAreaAsync(CreateCommonAreaDto dto);
        Task<ResponseDto> UpdateCommonAreaAsync(int id, CreateCommonAreaDto dto);
        Task<ResponseDto> DeleteCommonAreaAsync(int id);
    }
}