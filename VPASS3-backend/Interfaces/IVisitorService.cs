using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.Visitors;

namespace VPASS3_backend.Services
{
    public interface IVisitorService
    {
        Task<ResponseDto> GetAllVisitorsAsync();
        Task<ResponseDto> GetVisitorByIdAsync(int id);
        Task<ResponseDto> GetVisitorByIdentificationNumberAsync(string identificationNumber);
        Task<ResponseDto> CreateVisitorAsync(VisitorDto visitorDto);
        Task<ResponseDto> UpdateVisitorAsync(int id, VisitorDto visitorDto);
        Task<ResponseDto> DeleteVisitorAsync(int id);
    }
}
