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
    public class ReservableCommonAreaReservationController : ControllerBase
    {
        private readonly IReservableCommonAreaReservationService _reservationService;

        public ReservableCommonAreaReservationController(IReservableCommonAreaReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        [Audit("Creación de reserva en área común")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateReservableCommonAreaReservationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));

            var response = await _reservationService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _reservationService.GetAllAsync();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _reservationService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] CreateReservableCommonAreaReservationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _reservationService.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        [Audit("Eliminación de reserva en área común")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _reservationService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}