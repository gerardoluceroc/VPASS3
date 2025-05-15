using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.Models;
using VPASS3_backend.Services;
using System.Threading.Tasks;
using VPASS3_backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Filters;

namespace VPASS3_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IUserContextService _userContext;

        public UserController(UserService userService, IUserContextService userContext)
        {
            _userService = userService;
            _userContext = userContext;
        }


        [HttpPost("create")]
        [Audit("Creación de usuario")]
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

        [Authorize(Policy = "ManageEverything")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var responseDto = await _userService.GetAllUsersAsync();
            return StatusCode(responseDto.StatusCode, responseDto);
        }


        [Authorize(Policy = "ReadOnlyOwnProfile")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            // Se verifica que el usuario sea Super Admin o que esté consultando a recursos relacionados con su usuario
            if (!_userContext.CanAccessOwnResourceById(id))
            {
                return StatusCode(403, new ResponseDto(403, message: "No cuenta con los permisos para ver la información de otros usuarios"));
            }

            var responseDto = await _userService.GetUserByIdAsync(id);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Actualización de información de usuario")]
        [HttpPut("update")]
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

            // Se verifica que el usuario sea Super Admin o que esté consultando a recursos relacionados con su usuario
            if (!_userContext.CanAccessOwnResourceByEmail(userDto.Email))
            {
                return StatusCode(403, new ResponseDto(403, message: "No cuenta con los permisos para administar la información de otros usuarios"));
            }

            var responseDto = await _userService.UpdateUserAsync(userDto);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Eliminación de usuario")]
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Se verifica que el usuario sea Super Admin o que esté consultando a recursos relacionados con su usuario
            if (!_userContext.CanAccessOwnResourceById(id))
            {
                return StatusCode(403, new ResponseDto(403, message: "No cuenta con los permisos para administar la información de otros usuarios"));
            }

            var responseDto = await _userService.DeleteUserAsync(id);
            return StatusCode(responseDto.StatusCode, responseDto);
        }
    }

}

