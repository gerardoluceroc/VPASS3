using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.VisitTypes;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using VPASS3_backend.Models;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VisitTypeController : ControllerBase
    {
        private readonly IVisitTypeService _visitTypeService;

        public VisitTypeController(IVisitTypeService visitTypeService)
        {
            _visitTypeService = visitTypeService;
        }

        /// <summary>
        /// Crea un nuevo tipo de visita para un establecimiento específico.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// **Validaciones:**
        /// - Solo los usuarios con permisos para administrar el establecimiento pueden crear tipos de visita en él.
        /// - El `IdEstablishment` proporcionado debe corresponder a un establecimiento existente.
        /// - El `Name` del tipo de visita debe ser único dentro del mismo establecimiento (la comparación no distingue mayúsculas de minúsculas).
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para VisitTypeDto:**
        /// ```json
        /// {
        ///   "name": "Técnico de Internet",
        ///   "idEstablishment": 1
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para la creación del tipo de visita.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`Name` (string, requerido):** El nombre del tipo de visita (ej., "Visita Normal", "Gasfiter", "VTR").
        /// - **`IdEstablishment` (int, requerido):** El ID del establecimiento al que pertenecerá este tipo de visita.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="VisitType"/> creado (incluyendo su Id generado automáticamente, Name, IdEstablishment) y 'Message' es "Tipo de visita creado correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos. Verifica los campos ingresados." si el DTO no es válido.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para crear tipos de visita en este establecimiento." si el usuario no tiene los permisos para el establecimiento especificado.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "El establecimiento indicado no existe." si el `IdEstablishment` no existe.</response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "Ya existe un tipo de visita con el nombre '{nombre}' para este establecimiento." si ya existe un tipo de visita con el mismo nombre en el mismo establecimiento.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al crear el tipo de visita." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] VisitTypeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));

            var response = await _visitTypeService.CreateVisitTypeAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene una lista de todos los tipos de visita registrados en el sistema,
        /// filtrados según los permisos del usuario autenticado.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Si el usuario es **SUPERADMIN**, la respuesta incluirá todos los tipos de visita de todos los establecimientos.
        /// - Si el usuario es **ADMIN**, solo se devolverán los tipos de visita que pertenecen a su establecimiento asociado.
        ///   La lógica de `_userContext.CanAccessVisitType(vt)` garantiza que el administrador solo acceda a los tipos de visita de su propio contexto.
        /// </remarks>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos <see cref="VisitType"/> y 'Message' es "Tipos de visita obtenidos correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes un establecimiento asociado." si el usuario no es SUPERADMIN y no tiene un ID de establecimiento asignado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener los tipos de visita." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _visitTypeService.GetAllVisitTypesAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene los detalles de un tipo de visita específico buscando por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Los usuarios con rol **SUPERADMIN** pueden acceder a cualquier tipo de visita por su ID.
        /// - Los usuarios con rol **ADMIN** solo pueden ver los tipos de visita que pertenecen a su establecimiento asociado.
        ///   La lógica de `_userContext.CanAccessVisitType(visitType)` garantiza que el administrador solo acceda a los tipos de visita de su propio contexto.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del tipo de visita a recuperar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="VisitType"/> completo y 'Message' es "Tipo de visita obtenido correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para acceder a este tipo de visita." si el usuario no tiene los permisos para el tipo de visita.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Tipo de visita no encontrado." si el ID proporcionado no corresponde a un tipo de visita existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener el tipo de visita." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _visitTypeService.GetVisitTypeByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Actualiza la información de un tipo de visita existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// **Validaciones y restricciones:**
        /// - Solo los usuarios con permisos para administrar el establecimiento (`_userContext.CanAccessVisitType`) pueden modificar sus tipos de visita.
        /// - Un usuario `ADMIN` **no puede reasignar** un tipo de visita a un establecimiento diferente; esta operación está restringida para `SUPERADMIN`.
        /// - Se valida que el `IdEstablishment` (si se cambia) corresponda a un establecimiento existente.
        /// - El `Name` del tipo de visita debe ser único dentro del establecimiento al que pertenecerá. La comparación no distingue mayúsculas de minúsculas.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para VisitTypeDto:**
        /// ```json
        /// {
        ///   "name": "Técnico de Redes", // Nuevo nombre para el tipo de visita
        ///   "idEstablishment": 1        // El ID del establecimiento (puede ser el mismo o diferente si eres SUPERADMIN)
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">El **ID** del tipo de visita que se desea actualizar.</param>
        /// <param name="dto">
        /// **Objeto DTO con los datos actualizados del tipo de visita.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`Name` (string, requerido):** El nuevo nombre para el tipo de visita.
        /// - **`IdEstablishment` (int, requerido):** El ID del establecimiento al que pertenece (o se reasignará) este tipo de visita.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="VisitType"/> actualizado y 'Message' es "Tipo de visita actualizado correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos. Verifica los campos ingresados." si el DTO no es válido.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "No tienes permiso para editar este tipo de visita." si el usuario no tiene los permisos para el tipo de visita.
        /// - "No puedes reasignar el tipo de visita a otro establecimiento." si un usuario ADMIN intenta cambiar el establecimiento.
        /// </response>
        /// <response code="404">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "Tipo de visita no encontrado." si el `id` proporcionado no corresponde a un tipo de visita existente.
        /// - "El nuevo establecimiento indicado no existe." si el `IdEstablishment` proporcionado no existe.
        /// </response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "Ya existe un tipo de visita con el nombre '{nombre}' para este establecimiento." si el nuevo nombre ya está en uso en el mismo establecimiento.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al actualizar el tipo de visita." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] VisitTypeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));

            var response = await _visitTypeService.UpdateVisitTypeAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Elimina un tipo de visita del sistema de forma física por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// **Reglas de eliminación:**
        /// - Solo los usuarios con permisos para administrar el establecimiento (`_userContext.CanAccessVisitType`) pueden eliminar sus tipos de visita.
        /// - La eliminación es **física**, lo que significa que el registro del tipo de visita será removido permanentemente de la base de datos.
        /// - Se registra la acción de eliminación en el log de auditoría.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del tipo de visita a eliminar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Tipo de visita eliminado correctamente." si la operación fue exitosa.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para eliminar este tipo de visita." si el usuario no tiene los permisos para el tipo de visita.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Tipo de visita no encontrado." si el ID proporcionado no corresponde a un tipo de visita existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al eliminar el tipo de visita." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _visitTypeService.DeleteVisitTypeAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
