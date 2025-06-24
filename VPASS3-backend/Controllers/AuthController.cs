using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VPASS3_backend.DTOs;
using VPASS3_backend.Services;

namespace VPASS3_backend.Controllers
{
    /// <summary>
    /// Controlador para autenticación y sesión de usuarios.
    /// </summary>
    /// 

//    [HttpPost("login")]
//    [SwaggerOperation(
//    Summary = "Inicia sesión de un usuario y devuelve un token de autenticación.",
//    Description = "Este endpoint permite a los usuarios autenticarse con su correo electrónico y contraseña. Retorna una respuesta con el token JWT si la autenticación es exitosa."
//)]
//    [SwaggerResponse(200, "Retorna un token JWT y los datos del usuario si las credenciales son válidas.", typeof(ApiResponse<LoginResponseDto>))]
//    [SwaggerResponse(400, "Si los datos de entrada son inválidos (ej. DTO nulo o campos faltantes).")]
//    [SwaggerResponse(401, "Si las credenciales son incorrectas.")]
//    [SwaggerResponse(500, "Si ocurre un error interno del servidor.")]
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Inicia sesión de un usuario y devuelve un token de autenticación.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite a los usuarios autenticarse con su correo electrónico y contraseña.
        /// Retorna un ResponseDto con el token JWT en la propiedad 'Data' si la autenticación es exitosa.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body):**
        /// ```json
        /// {
        ///   "email": "usuario@ejemplo.com",
        ///   "password": "UnaContraseñaSegura123!"
        /// }
        /// ```
        /// </remarks>
        /// <param name="loginDto">
        /// **Objeto DTO de login que contiene las credenciales del usuario.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`email` (string, requerido):** El correo electrónico del usuario.
        /// - **`password` (string, requerido):** La contraseña del usuario.
        /// </param>
        /// <returns>Un <see cref="IActionResult"/> que representa el resultado de la operación de inicio de sesión.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene un objeto { "Token": "cadena_jwt" } si las credenciales son válidas.</response>
        /// <response code="400">Retorna un ResponseDto con un mensaje de error si los datos de entrada son inválidos.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si las credenciales son incorrectas.</response>
        /// <response code="500">Retorna un ResponseDto con un mensaje de error si ocurre un error interno del servidor.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

            // Se retorna el resultado con el mensaje adecuado
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Registro de cierre de sesión de un usuario.
        /// </summary>
        /// <remarks>
        /// Este endpoint es mas que nada para registrar en el sistema que un usuario ha cerrado sesión.
        /// No tiene ningun otro tipo de impacto, no invalida el token ni nada por el estilo. Solo sirve para notificarlo en caso de que el usuario cierre sesión en el frontend.
        /// Para identificar el usuario que ha cerrado sesión, se usa el claim nameIdentifier que viene en el token al hacer la petición.
        ///
        /// </remarks>
        /// <response code="200">Retorna un ResponseDto con el codigo 200 y el mensaje correspondiente.</response>
        /// <response code="500">Retorna un ResponseDto con un mensaje de error si ocurre un error interno del servidor.</response>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var response = await _authService.LogoutAsync(User);
            return StatusCode(response.StatusCode, response);
        }
    }
}