using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.DTOs;
using VPASS3_backend.Filters;
using VPASS3_backend.Interfaces;

namespace VPASS3_backend.Controllers.CommonAreaControllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommonAreaController : ControllerBase
    {
        private readonly ICommonAreaService _commonAreaService;
        private readonly IUserContextService _userContext;

        public CommonAreaController(ICommonAreaService commonAreaService, IUserContextService userContext)
        {
            _commonAreaService = commonAreaService;
            _userContext = userContext;
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateCommonAreaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));

            var response = await _commonAreaService.CreateCommonAreaAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _commonAreaService.GetAllCommonAreasAsync();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _commonAreaService.GetCommonAreaByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] CreateCommonAreaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));

            var response = await _commonAreaService.UpdateCommonAreaAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _commonAreaService.DeleteCommonAreaAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
