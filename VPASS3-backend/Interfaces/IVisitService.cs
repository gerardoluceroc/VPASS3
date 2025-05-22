using VPASS3_backend.DTOs.Visits;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Interfaces
{
    public interface IVisitService
    {
        Task<ResponseDto> GetAllVisitsAsync();
        Task<ResponseDto> GetVisitByIdAsync(int id);
        Task<ResponseDto> CreateVisitAsync(VisitDto visitDto);
        Task<ResponseDto> UpdateVisitAsync(int id, VisitDto visitDto);
        Task<ResponseDto> DeleteVisitAsync(int id);

        Task<ResponseDto> ExportVisitsToExcelAsync(GetVisitByDatesDto dto);
    }
}
