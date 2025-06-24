using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.Directions;
using VPASS3_backend.Interfaces;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DirectionController : ControllerBase
    {
        private readonly IDirectionService _directionService;

        public DirectionController(IDirectionService directionService)
        {
            _directionService = directionService;
        }

        /// <summary>
        /// Crea una nueva dirección de visita (sentido) en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **SUPERADMIN** (política "ManageEverything").
        /// Permite definir nuevos "sentidos" de visita, como "Entrada" o "Salida".
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para DirectionDto:**
        /// ```json
        /// {
        ///   "visitDirection": "Entrada"
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para la creación de la dirección.**
        /// Se espera un objeto JSON con la siguiente propiedad:
        /// - **`visitDirection` (string, requerido):** El nombre o descripción del sentido de la visita (ej., "Entrada", "Salida").
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto Direction creado (incluyendo su Id generado automáticamente), y 'Message' es "Sentido creado correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos. Verifica los campos ingresados." si el DTO no es válido.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene el rol SUPERADMIN.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al crear el sentido." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageEverything")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] DirectionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _directionService.CreateDirectionAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene una lista de todas las direcciones de visita (sentidos) registradas.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Retorna una lista de objetos Direction en la propiedad 'Data'.
        /// Cada objeto Direction incluye su Id (int) y VisitDirection (string).
        /// </remarks>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos Direction (Id, VisitDirection), y 'Message' es "Sentidos obtenidos correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los roles ADMIN o SUPERADMIN.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener los sentidos." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _directionService.GetAllDirectionsAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene los detalles de una dirección de visita (sentido) específica por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Retorna un ResponseDto con el objeto Direction en la propiedad 'Data' si es encontrado.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) de la dirección a consultar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene un objeto Direction con sus detalles (Id, VisitDirection), y 'Message' es "Sentido obtenido correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los roles ADMIN o SUPERADMIN.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Sentido no encontrado." si la dirección con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener el sentido." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _directionService.GetDirectionByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Actualiza la descripción de una dirección de visita (sentido) existente por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **SUPERADMIN** (política "ManageEverything").
        /// Permite modificar la descripción del sentido de la visita.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para DirectionDto:**
        /// ```json
        /// {
        ///   "visitDirection": "Nuevo Sentido de Visita"
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) de la dirección a actualizar.</param>
        /// <param name="dto">
        /// **Objeto DTO con los datos actualizados para la dirección.**
        /// Se espera un objeto JSON con la siguiente propiedad:
        /// - **`visitDirection` (string, requerido):** La nueva descripción para el sentido de la visita.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto Direction actualizado, y 'Message' es "Sentido actualizado correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos. Verifica los campos ingresados." si el DTO no es válido.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene el rol SUPERADMIN.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Sentido no encontrado." si la dirección con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al actualizar el sentido." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageEverything")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] DirectionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _directionService.UpdateDirectionAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Elimina una dirección de visita (sentido) existente por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **SUPERADMIN** (política "ManageEverything").
        /// Permite la eliminación permanente de una dirección de visita.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) de la dirección a eliminar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Sentido eliminado correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene el rol SUPERADMIN.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Sentido no encontrado." si la dirección con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al eliminar el sentido." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageEverything")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _directionService.DeleteDirectionAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}