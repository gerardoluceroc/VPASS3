using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.ApartmentOwnerships;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApartmentOwnershipController : ControllerBase
    {
        private readonly IApartmentOwnershipService _apartmentOwnershipService;
        private readonly IUserContextService _userContext;

        public ApartmentOwnershipController(IApartmentOwnershipService apartmentOwnershipService, IUserContextService userContext)
        {
            _apartmentOwnershipService = apartmentOwnershipService;
            _userContext = userContext;
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateApartmentOwnershipDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _apartmentOwnershipService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("end")]
        public async Task<ActionResult<ResponseDto>> EndOwnership([FromBody] EndApartmentOwnershipDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _apartmentOwnershipService.EndCurrentOwnershipAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _apartmentOwnershipService.GetAllAsync();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _apartmentOwnershipService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _apartmentOwnershipService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
