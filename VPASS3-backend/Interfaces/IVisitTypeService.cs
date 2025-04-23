using VPASS3_backend.DTOs.VisitTypes;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces
{
    public interface IVisitTypeService
    {
        Task<ResponseDto> GetAllVisitTypesAsync();
        Task<ResponseDto> GetVisitTypeByIdAsync(int id);
        Task<ResponseDto> CreateVisitTypeAsync(VisitTypeDto dto);
        Task<ResponseDto> UpdateVisitTypeAsync(int id, VisitTypeDto dto);
        Task<ResponseDto> DeleteVisitTypeAsync(int id);
    }
}
