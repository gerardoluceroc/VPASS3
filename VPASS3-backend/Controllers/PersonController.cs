using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.Persons;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Services;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPersons()
        {
            var response = await _personService.GetAllPersonsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonById(int id)
        {
            var response = await _personService.GetPersonByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("idnumber/{identificationNumber}")]
        public async Task<IActionResult> GetPersonByIdentificationNumber(string identificationNumber)
        {
            var response = await _personService.GetPersonByIdentificationNumberAsync(identificationNumber);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePerson([FromBody] PersonDto dto)
        {
            var response = await _personService.CreatePersonAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] PersonDto dto)
        {
            var response = await _personService.UpdatePersonAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var response = await _personService.DeletePersonAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}

