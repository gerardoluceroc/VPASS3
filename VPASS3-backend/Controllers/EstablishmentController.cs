using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.Establishments;
using VPASS3_backend.Interfaces;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EstablishmentController : ControllerBase
    {
        private readonly IEstablishmentService _establishmentService;

        public EstablishmentController(IEstablishmentService establishmentService)
        {
            _establishmentService = establishmentService;
        }

        [Authorize(Policy = "ManageEverything")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateEstablishmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _establishmentService.CreateEstablishmentAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageEverything")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _establishmentService.GetAllEstablishmentsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _establishmentService.GetEstablishmentByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] CreateEstablishmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _establishmentService.UpdateEstablishmentAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {

            var response = await _establishmentService.DeleteEstablishmentAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}




























//namespace VPASS3_backend.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class EstablishmentController : ControllerBase
//    {
//        private readonly IEstablishmentService _establishmentService;

//        public EstablishmentController(IEstablishmentService establishmentService)
//        {
//            _establishmentService = establishmentService;
//        }

//        [HttpPost]
//        public async Task<IActionResult> CreateEstablishment([FromBody] CreateEstablishmentDto createEstablishmentDto)
//        {
//            if (!ModelState.IsValid)
//            {
//                var errores = ModelState.Values.SelectMany(v => v.Errors)
//                                               .Select(e => e.ErrorMessage)
//                                               .ToList();

//                return StatusCode(400, new ResponseDto(400, message: "Error de validación.", data: errores));
//            }

//            var response = await _establishmentService.CreateEstablishmentAsync(createEstablishmentDto);
//            return StatusCode(response.StatusCode, response);
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAllEstablishments()
//        {
//            var response = await _establishmentService.GetAllEstablishmentsAsync();
//            return StatusCode(response.StatusCode, response);
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetEstablishmentById(int id)
//        {
//            var response = await _establishmentService.GetEstablishmentByIdAsync(id);
//            return StatusCode(response.StatusCode, response);
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateEstablishment(int id, [FromBody] CreateEstablishmentDto createEstablishmentDto)
//        {
//            if (!ModelState.IsValid)
//            {
//                var errores = ModelState.Values.SelectMany(v => v.Errors)
//                                               .Select(e => e.ErrorMessage)
//                                               .ToList();

//                return StatusCode(400, new ResponseDto(400, message: "Error de validación.", data: errores));
//            }

//            var response = await _establishmentService.UpdateEstablishmentAsync(id, createEstablishmentDto);
//            return StatusCode(response.StatusCode, response);
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteEstablishment(int id)
//        {
//            var response = await _establishmentService.DeleteEstablishmentAsync(id);
//            return StatusCode(response.StatusCode, response);
//        }
//    }


//}
