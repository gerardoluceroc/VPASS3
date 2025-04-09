using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.Services;

namespace VPASS3_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

            // Se retorna el resultado con el mensaje adecuado
            return StatusCode(response.StatusCode, response);
        }
    }
}