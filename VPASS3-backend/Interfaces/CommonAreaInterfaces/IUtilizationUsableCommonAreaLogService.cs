using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces.CommonAreaInterfaces
{
    public interface IUtilizationUsableCommonAreaLogService
    {
        Task<ResponseDto> CreateAsync(CreateUtilizationUsableCommonAreaLogDto dto);
        Task<ResponseDto> GetAllAsync();
        Task<ResponseDto> GetByIdAsync(int id);
        Task<ResponseDto> UpdateAsync(int id, CreateUtilizationUsableCommonAreaLogDto dto);
        Task<ResponseDto> DeleteAsync(int id);
    }
}
