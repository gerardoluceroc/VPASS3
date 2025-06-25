using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using VPASS3_backend.Filters;
using VPASS3_backend.DTOs.Apartments;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApartmentController : ControllerBase
    {
        private readonly IApartmentService _apartmentService;

        public ApartmentController(IApartmentService apartmentService)
        {
            _apartmentService = apartmentService;
        }

        [HttpPost("create")]
        [Audit("Creación de departamento")]
        [Authorize(Policy = "ManageOwnProfile")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] ApartmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _apartmentService.CreateApartmentAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("all")]
        [Authorize(Policy = "ManageOwnProfile")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _apartmentService.GetAllApartmentsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ManageOwnProfile")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _apartmentService.GetApartmentByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("update/{id}")]
        [Audit("Actualización de información de departamento")]
        [Authorize(Policy = "ManageOwnProfile")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] ApartmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _apartmentService.UpdateApartmentAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("delete/{id}")]
        [Audit("Eliminación de departamento")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _apartmentService.DeleteApartmentAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}