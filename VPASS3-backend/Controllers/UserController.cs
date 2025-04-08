using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.Models;
using VPASS3_backend.Services;
using System.Threading.Tasks;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
                var response = new ResponseDto(400, message: "Error de validación.");
                response.Data = errores;
                return StatusCode(response.StatusCode, response);
            }

            // Se llama al servicio para crear el usuario y asignarle el rol
            var responseDto = await _userService.CreateUserAsync(userDto);

            // Se retorna el resultado con el mensaje adecuado
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var responseDto = await _userService.GetAllUsersAsync();
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var responseDto = await _userService.GetUserByIdAsync(id);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] CreateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
                var response = new ResponseDto(400, message: "Error de validación.");
                response.Data = errores;
                return StatusCode(response.StatusCode, response);
            }

            var responseDto = await _userService.UpdateUserAsync(userDto);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var responseDto = await _userService.DeleteUserAsync(id);
            return StatusCode(responseDto.StatusCode, responseDto);
        }
    }

}

