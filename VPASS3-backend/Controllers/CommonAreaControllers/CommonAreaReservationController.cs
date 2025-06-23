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
    public class CommonAreaReservationController : ControllerBase
    {
        private readonly ICommonAreaReservationService _svc;
        private readonly IUserContextService _userCtx;

        public CommonAreaReservationController(ICommonAreaReservationService svc, IUserContextService userCtx)
        {
            _svc = svc;
            _userCtx = userCtx;
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateCommonAreaReservationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Revisa los campos."));

            var resp = await _svc.CreateAsync(dto);
            return StatusCode(resp.StatusCode, resp);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var resp = await _svc.GetAllAsync();
            return StatusCode(resp.StatusCode, resp);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var resp = await _svc.GetByIdAsync(id);
            return StatusCode(resp.StatusCode, resp);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] UpdateCommonAreaReservationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Revisa los campos."));

            var resp = await _svc.UpdateAsync(id, dto);
            return StatusCode(resp.StatusCode, resp);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var resp = await _svc.DeleteAsync(id);
            return StatusCode(resp.StatusCode, resp);
        }
    }

}