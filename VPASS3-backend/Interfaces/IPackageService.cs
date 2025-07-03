using VPASS3_backend.DTOs.Packages;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces
{
    public interface IPackageService
    {
        Task<ResponseDto> CreateAsync(CreatePackageDto dto);
        Task<ResponseDto> MarkAsDeliveredAsync(ReceivePackageDto dto);
        Task<ResponseDto> UpdateAsync(UpdatePackageDto dto);
        Task<ResponseDto> GetAllAsync();
        Task<ResponseDto> GetByIdAsync(int id);
        Task<ResponseDto> DeleteAsync(int id);
    }
}