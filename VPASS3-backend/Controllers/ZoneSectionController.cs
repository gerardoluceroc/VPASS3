using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.ZoneSections;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using VPASS3_backend.Filters;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZoneSectionController : ControllerBase
    {
        private readonly IZoneSectionService _zoneSectionService;

        public ZoneSectionController(IZoneSectionService zoneSectionService)
        {
            _zoneSectionService = zoneSectionService;
        }

        [HttpPost("create")]
        [Audit("Creación de subzona")]
        [Authorize(Policy = "ManageOwnProfile")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] ZoneSectionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _zoneSectionService.CreateZoneSectionAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("all")]
        [Authorize(Policy = "ManageOwnProfile")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _zoneSectionService.GetAllZoneSectionsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ManageOwnProfile")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _zoneSectionService.GetZoneSectionByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("update/{id}")]
        [Audit("Actualización de información de Subzona")]
        [Authorize(Policy = "ManageOwnProfile")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] ZoneSectionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _zoneSectionService.UpdateZoneSectionAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("delete/{id}")]
        [Audit("Eliminación de Subzona")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _zoneSectionService.DeleteZoneSectionAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}