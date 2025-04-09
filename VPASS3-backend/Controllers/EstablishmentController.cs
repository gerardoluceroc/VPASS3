using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.Establishments;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstablishmentController : ControllerBase
    {
        private readonly IEstablishmentService _establishmentService;

        public EstablishmentController(IEstablishmentService establishmentService)
        {
            _establishmentService = establishmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEstablishment([FromBody] CreateEstablishmentDto createEstablishmentDto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();

                return StatusCode(400, new ResponseDto(400, message: "Error de validación.", data: errores));
            }

            var response = await _establishmentService.CreateEstablishmentAsync(createEstablishmentDto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEstablishments()
        {
            var response = await _establishmentService.GetAllEstablishmentsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEstablishmentById(int id)
        {
            var response = await _establishmentService.GetEstablishmentByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEstablishment(int id, [FromBody] CreateEstablishmentDto createEstablishmentDto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();

                return StatusCode(400, new ResponseDto(400, message: "Error de validación.", data: errores));
            }

            var response = await _establishmentService.UpdateEstablishmentAsync(id, createEstablishmentDto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstablishment(int id)
        {
            var response = await _establishmentService.DeleteEstablishmentAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }


}
