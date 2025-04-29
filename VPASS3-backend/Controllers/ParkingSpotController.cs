using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.ParkingSpots;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParkingSpotController : ControllerBase
    {
        private readonly IParkingSpotService _service;

        public ParkingSpotController(IParkingSpotService service)
        {
            _service = service;
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _service.GetAllParkingSpotsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _service.GetParkingSpotByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] ParkingSpotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, "Datos inválidos."));

            var response = await _service.CreateParkingSpotAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] ParkingSpotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, "Datos inválidos."));

            var response = await _service.UpdateParkingSpotAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _service.DeleteParkingSpotAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
