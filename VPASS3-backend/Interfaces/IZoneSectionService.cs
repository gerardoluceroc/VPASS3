using VPASS3_backend.DTOs.ZoneSections;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces
{
    public interface IZoneSectionService
    {
        Task<ResponseDto> GetAllZoneSectionsAsync();
        Task<ResponseDto> GetZoneSectionByIdAsync(int id);
        Task<ResponseDto> CreateZoneSectionAsync(ZoneSectionDto dto);
        Task<ResponseDto> UpdateZoneSectionAsync(int id, ZoneSectionDto dto);
        Task<ResponseDto> DeleteZoneSectionAsync(int id);
    }
}
