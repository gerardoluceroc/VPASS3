using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.Filters;
using VPASS3_backend.Interfaces.CommonAreaInterfaces;

namespace VPASS3_backend.Controllers.CommonAreaControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UtilizationUsableCommonAreaLogController : ControllerBase
    {
        private readonly IUtilizationUsableCommonAreaLogService _service;

        public UtilizationUsableCommonAreaLogController(IUtilizationUsableCommonAreaLogService service)
        {
            _service = service;
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateUtilizationUsableCommonAreaLogDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));

            var response = await _service.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _service.GetAllAsync();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] CreateUtilizationUsableCommonAreaLogDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));

            var response = await _service.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _service.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}