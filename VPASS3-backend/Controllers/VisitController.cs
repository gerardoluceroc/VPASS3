using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.Visits;
using VPASS3_backend.Filters;
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

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllVisits()
        {
            var response = await _visitService.GetAllVisitsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVisitById(int id)
        {
            var response = await _visitService.GetVisitByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Registro de visita")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateVisit([FromBody] VisitDto dto)
        {
            var response = await _visitService.CreateVisitAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Actualización de información de visita")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateVisit(int id, [FromBody] VisitDto dto)
        {
            var response = await _visitService.UpdateVisitAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Eliminación de visita")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVisit(int id)
        {
            var response = await _visitService.DeleteVisitAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Descarga de visitas en Excel")]
        [HttpPost("export/excel")]
        public async Task<IActionResult> ExportVisitsToExcel([FromBody] GetVisitByDatesDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica las fechas ingresadas."));
            }

            var response = await _visitService.ExportVisitsToExcelAsync(dto);

            if (response.StatusCode != 200 || response.Data == null)
            {
                return StatusCode(response.StatusCode, response);
            }

            // Extraer los datos del archivo del response
            var fileData = (dynamic)response.Data;
            byte[] fileContents = fileData.FileContent;
            string contentType = fileData.ContentType;
            string fileName = fileData.FileName;

            return File(fileContents, contentType, fileName);
        }
    }
}