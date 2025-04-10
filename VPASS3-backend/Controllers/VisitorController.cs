using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.Visitors;
using VPASS3_backend.Services;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VisitorController : ControllerBase
    {
        private readonly IVisitorService _visitorService;

        public VisitorController(IVisitorService visitorService)
        {
            _visitorService = visitorService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllVisitors()
        {
            var response = await _visitorService.GetAllVisitorsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVisitorById(int id)
        {
            var response = await _visitorService.GetVisitorByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateVisitor([FromBody] VisitorDto dto)
        {
            var response = await _visitorService.CreateVisitorAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateVisitor(int id, [FromBody] VisitorDto dto)
        {
            var response = await _visitorService.UpdateVisitorAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVisitor(int id)
        {
            var response = await _visitorService.DeleteVisitorAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}