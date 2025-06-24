using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.Establishments;
using VPASS3_backend.Filters;
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

        /// <summary>
        /// Crea un nuevo establecimiento en el sistema y lo asocia a un usuario existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario que realiza la solicitud tenga el rol **SUPERADMIN** (política "ManageEverything").
        /// El establecimiento se creará y se vinculará al usuario cuyo correo electrónico se proporcione en el DTO.
        /// El usuario especificado no debe estar ya asociado a otro establecimiento.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para CreateEstablishmentDto:**
        /// ```json
        /// {
        ///   "name": "Edificio Central",
        ///   "email": "admin.edificio@ejemplo.com"
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para la creación del establecimiento y la asociación del usuario.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`name` (string, requerido):** El nombre del nuevo establecimiento.
        /// - **`email` (string, requerido):** El correo electrónico del usuario existente que será asociado al establecimiento.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto Establishment creado (Id, Name), y 'Message' es "Establecimiento creado correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos. Verifica los campos ingresados." si el DTO no es válido. 'Data' puede contener una lista de errores de validación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene el rol SUPERADMIN.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "No se encontró ningún usuario con el email ingresado." si el email proporcionado no corresponde a un usuario existente.</response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "El usuario ya está asociado a un establecimiento y no puede ser asignado a otro." si el usuario ya tiene un establecimiento asignado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al crear el establecimiento." si ocurre un error interno no controlado.</response>
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

        /// <summary>
        /// Obtiene una lista de todos los establecimientos registrados en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario que realiza la solicitud tenga el rol **SUPERADMIN** (política "ManageEverything").
        /// Retorna una lista completa de objetos Establishment, incluyendo sus relaciones con Users, Zones (y sus ZoneSections), ParkingSpots, Blacklists y CommonAreas.
        /// </remarks>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos Establishment. Cada Establishment incluye sus propiedades (Id, Name) y sus colecciones relacionadas (Users, Zones, ParkingSpots, Blacklists, CommonAreas).</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene el rol SUPERADMIN.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener los establecimientos." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageEverything")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _establishmentService.GetAllEstablishmentsAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene los detalles de un establecimiento específico por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario que realiza la solicitud tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Los usuarios solo pueden ver la información de los establecimientos a los que están asociados, a menos que tengan el rol SUPERADMIN.
        /// Retorna un ResponseDto con un objeto Establishment en la propiedad 'Data' si es encontrado, incluyendo sus relaciones con Users, Zones, ParkingSpots, Blacklists y CommonAreas.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del establecimiento a consultar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene un objeto Establishment con sus propiedades (Id, Name) y sus colecciones relacionadas (Users, Zones, ParkingSpots, Blacklists, CommonAreas), y 'Message' es "Establecimiento obtenido correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos para ver la información del establecimiento solicitado.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Establecimiento no encontrado." si el establecimiento con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener el establecimiento." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _establishmentService.GetEstablishmentByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Actualiza la información de un establecimiento existente, incluyendo su nombre y la asociación a un usuario.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Los usuarios solo pueden actualizar establecimientos a los que están asociados, a menos que tengan el rol SUPERADMIN.
        /// Puedes actualizar el nombre del establecimiento y, opcionalmente, asignar un usuario diferente al establecimiento.
        /// Si se proporciona un email, el usuario con ese email será asociado a este establecimiento, siempre y cuando no esté ya asociado a otro establecimiento diferente.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para CreateEstablishmentDto (usado para actualización):**
        /// ```json
        /// {
        ///   "name": "Nuevo Nombre del Edificio",
        ///   "email": "nuevo.admin@ejemplo.com" // Opcional: para cambiar el usuario asociado
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del establecimiento a actualizar.</param>
        /// <param name="dto">
        /// **Objeto DTO con los datos actualizados del establecimiento.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`name` (string, requerido):** El nuevo nombre del establecimiento.
        /// - **`email` (string, opcional):** El correo electrónico de un usuario existente para asociarlo a este establecimiento.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto Establishment actualizado (Id, Name, y sus colecciones), y 'Message' es "Establecimiento actualizado correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos. Verifica los campos ingresados." si el DTO no es válido. 'Data' puede contener una lista de errores de validación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos para administrar la información del establecimiento solicitado.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Establecimiento no encontrado." si el ID no corresponde a un establecimiento existente, o "No se encontró ningún usuario con el email ingresado." si el email proporcionado no existe.</response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "El usuario ya está asociado a otro establecimiento." si se intenta asociar un usuario que ya tiene otro establecimiento asignado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al actualizar el establecimiento." si ocurre un error interno.</response>
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

        /// <summary>
        /// Elimina un establecimiento existente del sistema por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Los usuarios solo pueden eliminar establecimientos a los que están asociados, a menos que tengan el rol SUPERADMIN.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del establecimiento a eliminar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Establecimiento eliminado correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos para administrar la información del establecimiento solicitado.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Establecimiento no encontrado." si el establecimiento con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al eliminar el establecimiento." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {

            var response = await _establishmentService.DeleteEstablishmentAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}