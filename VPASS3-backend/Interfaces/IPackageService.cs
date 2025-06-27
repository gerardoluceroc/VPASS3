using VPASS3_backend.DTOs.Packages;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces
{
    public interface IPackageService
    {
        Task<ResponseDto> CreateAsync(CreatePackageDto dto);
    }
}

