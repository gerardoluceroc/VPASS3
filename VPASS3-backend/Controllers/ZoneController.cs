using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.Zones;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using VPASS3_backend.Filters;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZoneController : ControllerBase
    {
        private readonly IZoneService _zoneService;
        private readonly IUserContextService _userContext;

        public ZoneController(IZoneService zoneService, IUserContextService userContext)
        {
            _zoneService = zoneService;
            _userContext = userContext;
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Creación de zona")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateZoneDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            // Se verifica que el usuario sea Super Admin o que esté consultando a recursos relacionados con su usuario
            if (!_userContext.CanAccessOwnEstablishment(dto.EstablishmentId))
            {
                return StatusCode(403, new ResponseDto(403, message: "No cuenta con los permisos para administrar la información de otros usuarios"));
            }

            var response = await _zoneService.CreateZoneAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _zoneService.GetAllZonesAsync();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _zoneService.GetZoneByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Actualización de información de zona")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] CreateZoneDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _zoneService.UpdateZoneAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Eliminación de zona")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _zoneService.DeleteZoneAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}