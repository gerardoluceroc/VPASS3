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

        /// <summary>
        /// Crea un nuevo usuario en el sistema y le asigna un rol.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite registrar un nuevo usuario proporcionando su correo electrónico, contraseña y el ID del rol al que pertenecerá.
        /// La respuesta incluirá un <see cref="ResponseDto"/> que indica el resultado de la operación.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para CreateUserDto:**
        /// ```json
        /// {
        ///   "email": "nuevo.usuario@dominio.com",
        ///   "password": "PasswordSeguro123!",
        ///   "roleId": 1
        /// }
        /// ```
        /// </remarks>
        /// <param name="userDto">
        /// **Objeto DTO con los datos para la creación del usuario.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`email` (string, requerido):** El correo electrónico del nuevo usuario.
        /// - **`password` (string, requerido):** La contraseña inicial para el nuevo usuario.
        /// - **`roleId` (int, requerido):** El identificador único del rol que se asignará al usuario.
        /// </param>
        /// <returns>Un <see cref="IActionResult"/> que representa el resultado de la operación de creación del usuario.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el ID del usuario creado, y 'Message' es "Usuario creado con éxito.".</response>
        /// <response code="400">Retorna un  ResponseDto:
        ///     - Si los datos de entrada son inválidos, 'Data' contendrá una lista de mensajes de error de validación (List&lt;string&gt;).
        ///     - Si no se pudo asignar el rol o crear el usuario, 'Message' contendrá el error específico y 'Data' será nulo.
        /// </response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "El rol especificado no existe." si el RoleId no corresponde a un rol válido.</response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "El correo electrónico ya está registrado." si ya existe un usuario con ese email.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor." si ocurre un error interno no controlado.</response>
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

        /// <summary>
        /// Obtiene una lista de todos los usuarios registrados en el sistema, incluyendo sus roles.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere autenticación con una política de autorización "ManageEverything".
        /// Retorna una lista de objetos GetUserDto, donde cada objeto contiene los detalles de un usuario
        /// y una lista de los roles asociados a dicho usuario.
        /// </remarks>
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        /// <response code="200">
        /// Retorna un ResponseDto donde 'Data' contiene una **lista de GetUserDto**.
        /// Cada GetUserDto incluye: Id, UserName, Email, y una lista de Roles (Id, Name, NormalizedName, ConcurrencyStamp).
        /// </response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos necesarios (política "ManageEverything").</response>
        /// <response code="500">Retorna un ResponseDto con un mensaje de error si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageEverything")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var responseDto = await _userService.GetAllUsersAsync();
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        /// <summary>
        /// Obtiene los detalles de un usuario específico por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere autenticación y una política de autorización "ReadOnlyOwnProfile".
        /// Los usuarios solo pueden ver su propio perfil, a menos que tengan permisos de Super Admin.
        /// Retorna un ResponseDto con un GetUserDto en la propiedad 'Data' si el usuario es encontrado.
        /// </remarks>
        /// <param name="id">El identificador único (ID) del usuario a consultar.</param>
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene un GetUserDto con los detalles del usuario y sus roles asociados. GetUserDto incluye: Id, UserName, Email, y una lista de Roles (Id, Name, NormalizedName, ConcurrencyStamp).</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos necesarios o intenta acceder a un perfil ajeno sin ser Super Admin.</response>
        /// <response code="404">Retorna un ResponseDto con un mensaje de error si el usuario no es encontrado.</response>
        /// <response code="500">Retorna un ResponseDto con un mensaje de error si ocurre un error interno del servidor.</response>
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

        /// <summary>
        /// Actualiza la información de un usuario existente, incluyendo su correo electrónico, contraseña y rol.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere autenticación y una política de autorización "ManageOwnProfile".
        /// Permite a un usuario actualizar su propio perfil o, si tiene los permisos adecuados (e.g., Super Admin),
        /// la información de otros usuarios.
        /// La actualización incluye el email (que también es el UserName), la contraseña y, opcionalmente, el rol.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para CreateUserDto (usado para actualización):**
        /// ```json
        /// {
        ///   "email": "usuario.existente@dominio.com",
        ///   "password": "NuevaContraseñaSegura123!",
        ///   "roleId": 1 // Opcional si no se quiere cambiar el rol
        /// }
        /// ```
        /// </remarks>
        /// <param name="userDto">
        /// **Objeto DTO con los datos actualizados del usuario.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`email` (string, requerido):** El correo electrónico del usuario a actualizar. **Debe coincidir con un usuario existente.**
        /// - **`password` (string, requerido):** La nueva contraseña para el usuario.
        /// - **`roleId` (int, opcional):** El identificador único del nuevo rol a asignar al usuario. Si es nulo o inválido, el rol del usuario se mantiene.
        /// </param>
        /// <returns>Un IActionResult que representa el resultado de la operación de actualización.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Usuario actualizado con éxito.".</response>
        /// <response code="400">Retorna un ResponseDto:
        ///     - Si los datos de entrada son inválidos, 'Data' contendrá una lista de mensajes de error de validación (List&lt;string&gt;).
        ///     - Si no se pudo actualizar la contraseña o el usuario por otras razones, 'Message' contendrá el error y 'Data' será nulo.
        /// </response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos para administrar la información de otros usuarios.</response>
        /// <response code="404">Retorna un ResponseDto con un mensaje de error si el usuario con el correo electrónico proporcionado no es encontrado.</response>
        /// <response code="500">Retorna un ResponseDto con un mensaje de error si ocurre un error interno del servidor.</response>
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

        /// <summary>
        /// Elimina un usuario existente del sistema por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere autenticación y una política de autorización "ManageOwnProfile".
        /// Un usuario puede eliminarse a sí mismo, o un usuario con los permisos adecuados (e.g., Super Admin)
        /// puede eliminar a otros usuarios.
        /// </remarks>
        /// <param name="id">El identificador único (ID) del usuario a eliminar.</param>
        /// <returns>Un IActionResult que representa el resultado de la operación de eliminación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Usuario eliminado con éxito.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "No se pudo eliminar el usuario. Intente nuevamente." si la operación falla por razones internas.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos para administrar la información de otros usuarios.</response>
        /// <response code="404">Retorna un ResponseDto con un mensaje de error si el usuario con el ID proporcionado no es encontrado.</response>
        /// <response code="500">Retorna un ResponseDto con un mensaje de error si ocurre un error interno del servidor.</response>
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

