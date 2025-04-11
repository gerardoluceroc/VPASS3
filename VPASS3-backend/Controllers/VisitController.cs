using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.Visits;
using VPASS3_backend.Interfaces;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VisitController : ControllerBase
    {
        private readonly IVisitService _visitService;

        public VisitController(IVisitService visitService)
        {
            _visitService = visitService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllVisits()
        {
            var response = await _visitService.GetAllVisitsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVisitById(int id)
        {
            var response = await _visitService.GetVisitByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateVisit([FromBody] VisitDto dto)
        {
            var response = await _visitService.CreateVisitAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateVisit(int id, [FromBody] VisitDto dto)
        {
            var response = await _visitService.UpdateVisitAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVisit(int id)
        {
            var response = await _visitService.DeleteVisitAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}