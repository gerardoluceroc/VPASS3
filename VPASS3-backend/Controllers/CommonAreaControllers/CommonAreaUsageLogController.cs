using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces.CommonAreaInterfaces;
using VPASS3_backend.Interfaces;

namespace VPASS3_backend.Controllers.CommonAreaControllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommonAreaUsageLogController : ControllerBase
    {
        private readonly ICommonAreaUsageLogService _service;
        private readonly IUserContextService _userCtx;

        public CommonAreaUsageLogController(ICommonAreaUsageLogService service, IUserContextService userCtx)
        {
            _service = service;
            _userCtx = userCtx;
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateUsageLogDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));

            var resp = await _service.CreateUsageAsync(dto);
            return StatusCode(resp.StatusCode, resp);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var resp = await _service.GetAllAsync();
            return StatusCode(resp.StatusCode, resp);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var resp = await _service.GetByIdAsync(id);
            return StatusCode(resp.StatusCode, resp);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] UpdateUsageLogDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));

            var resp = await _service.UpdateAsync(id, dto);
            return StatusCode(resp.StatusCode, resp);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var resp = await _service.DeleteUsageAsync(id);
            return StatusCode(resp.StatusCode, resp);
        }
    }
}
