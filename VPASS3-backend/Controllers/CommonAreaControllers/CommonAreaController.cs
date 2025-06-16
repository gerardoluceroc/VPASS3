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
    public class CommonAreaController : ControllerBase
    {
        private readonly ICommonAreaService _service;
        private readonly IUserContextService _userContext;

        public CommonAreaController(ICommonAreaService service, IUserContextService userContext)
        {
            _service = service;
            _userContext = userContext;
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateCommonAreaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));
            var res = await _service.CreateAsync(dto);
            return StatusCode(res.StatusCode, res);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var res = await _service.GetAllAsync();
            return StatusCode(res.StatusCode, res);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var res = await _service.GetByIdAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] UpdateCommonAreaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));
            var res = await _service.UpdateAsync(id, dto);
            return StatusCode(res.StatusCode, res);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var res = await _service.DeleteAsync(id);
            return StatusCode(res.StatusCode, res);
        }
    }
}
