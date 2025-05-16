using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.Blacklist;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlacklistController : ControllerBase
    {
        private readonly IBlacklistService _blacklistService;
        private readonly IUserContextService _userContext;

        public BlacklistController(IBlacklistService blacklistService, IUserContextService userContext)
        {
            _blacklistService = blacklistService;
            _userContext = userContext;
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] BlacklistDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, "Datos inválidos."));

            if (!_userContext.CanAccessOwnEstablishment(dto.IdEstablishment))
                return StatusCode(403, new ResponseDto(403, message: "No tienes permiso para este establecimiento."));

            var response = await _blacklistService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }


        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _blacklistService.GetAllAsync();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _blacklistService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] BlacklistDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));

            var response = await _blacklistService.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _blacklistService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}