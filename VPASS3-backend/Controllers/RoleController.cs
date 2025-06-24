using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.Services;

namespace VPASS3_backend.Controllers
{
    // Controlador para roles
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Crea un nuevo rol en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite crear un nuevo rol proporcionando su nombre.
        /// El nombre del rol debe ser único.
        /// Retorna un ResponseDto indicando el resultado de la creación.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para RoleDto:**
        /// ```json
        /// {
        ///   "name": "SUPERADMIN"
        /// }
        /// ```
        /// </remarks>
        /// <param name="roleDto">
        /// **Objeto DTO con los datos del nuevo rol a crear.**
        /// Se espera un objeto JSON con la siguiente propiedad:
        /// - **`name` (string, requerido):** El nombre único del rol.
        /// </param>
        /// <returns>Un IActionResult que representa el resultado de la operación de creación del rol.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el ID del rol creado (int), y 'Message' es "Rol creado con éxito.".</response>
        /// <response code="400">Retorna un ResponseDto:
        ///     - Si los datos de entrada son inválidos (ej. el campo 'name' falta o es vacío), 'Data' contendrá una lista de mensajes de error de validación (List&lt;string&gt;).
        ///     - Si no se pudo crear el rol por razones internas, 'Message' contendrá "No se pudo crear el rol.".
        /// </response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "El rol ya existe." si ya existe un rol con el nombre proporcionado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor." si ocurre un error interno no controlado.</response>
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
                return BadRequest(new ResponseDto { StatusCode = 400, Message = "Error de validación.", Data = errores });
            }

            var response = await _roleService.CreateRoleAsync(roleDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene una lista de todos los roles disponibles en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere autenticación con una política de autorización "ManageEverything".
        /// Retorna un ResponseDto con una lista de objetos Role en la propiedad 'Data'.
        /// Cada objeto Role incluye propiedades como Id (int), Name (string), NormalizedName (string), y ConcurrencyStamp (string).
        /// </remarks>
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos Role. Cada Role incluye propiedades como Id (int), Name (string), NormalizedName (string), y ConcurrencyStamp (string).</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos necesarios (política "ManageEverything").</response>
        /// <response code="500">Retorna un ResponseDto con un mensaje de error si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageEverything")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await _roleService.GetAllRolesAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene los detalles de un rol específico por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere autenticación con una política de autorización "ManageEverything".
        /// Retorna un ResponseDto con un objeto Role en la propiedad 'Data' si el rol es encontrado.
        /// El objeto Role incluye propiedades como Id (int), Name (string), NormalizedName (string), y ConcurrencyStamp (string).
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del rol a consultar.</param>
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene un objeto Role con sus detalles (Id, Name, NormalizedName, ConcurrencyStamp).</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos necesarios (política "ManageEverything").</response>
        /// <response code="404">Retorna un ResponseDto con un mensaje de error si el rol con el ID proporcionado no es encontrado.</response>
        /// <response code="500">Retorna un ResponseDto con un mensaje de error si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageEverything")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var response = await _roleService.GetRoleByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Actualiza la información de un rol existente por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **SUPERADMIN** (política "ManageEverything").
        /// Permite modificar el nombre de un rol específico.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para RoleDto:**
        /// ```json
        /// {
        ///   "name": "ADMIN"
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del rol a actualizar.</param>
        /// <param name="roleDto">
        /// **Objeto DTO con los datos actualizados del rol.**
        /// Se espera un objeto JSON con la siguiente propiedad:
        /// - **`name` (string, requerido):** El nuevo nombre para el rol.
        /// </param>
        /// <returns>Un IActionResult que representa el resultado de la operación de actualización.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Rol actualizado con éxito.".</response>
        /// <response code="400">Retorna un ResponseDto:
        ///     - Si los datos de entrada son inválidos (ej. el campo 'name' falta o es vacío), 'Data' contendrá una lista de mensajes de error de validación (List&lt;string&gt;).
        ///     - Si no se pudo actualizar el rol por otras razones, 'Message' contendrá "No se pudo actualizar el rol.".
        /// </response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene el rol SUPERADMIN.</response>
        /// <response code="404">Retorna un ResponseDto con un mensaje de error si el rol con el ID proporcionado no es encontrado.</response>
        /// <response code="500">Retorna un ResponseDto con un mensaje de error si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageEverything")]
        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
                return BadRequest(new ResponseDto { StatusCode = 400, Message = "Error de validación.", Data = errores });
            }

            var response = await _roleService.UpdateRoleAsync(id, roleDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Elimina un rol existente del sistema por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **SUPERADMIN** (política "ManageEverything").
        /// Permite la eliminación permanente de un rol.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del rol a eliminar.</param>
        /// <returns>Un IActionResult que representa el resultado de la operación de eliminación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Rol eliminado con éxito.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "No se pudo eliminar el rol." si la operación falla por razones internas.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene el rol SUPERADMIN.</response>
        /// <response code="404">Retorna un ResponseDto con un mensaje de error si el rol con el ID proporcionado no es encontrado.</response>
        /// <response code="500">Retorna un ResponseDto con un mensaje de error si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageEverything")]
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var response = await _roleService.DeleteRoleAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
