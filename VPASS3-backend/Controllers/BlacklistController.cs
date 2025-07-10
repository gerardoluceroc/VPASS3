using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.Blacklist;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlacklistController : ControllerBase
    {
        private readonly IBlacklistService _blacklistService;
        private readonly IUserContextService _userContext;

        public BlacklistController(IBlacklistService blacklistService, IUserContextService userContext)
        {
            _blacklistService = blacklistService;
            _userContext = userContext;
        }

        /// <summary>
        /// Agrega una persona a la lista negra de un establecimiento específico.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Solo los usuarios con permisos para administrar el establecimiento pueden agregar personas a su lista negra.
        /// Una persona solo puede ser agregada una vez a la lista negra de un establecimiento determinado.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para BlacklistDto:**
        /// ```json
        /// {
        ///   "idPerson": 123,
        ///   "idEstablishment": 1,
        ///   "reason": "Comportamiento disruptivo recurrente"
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para agregar a la persona a la lista negra.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`idPerson` (int, requerido):** El ID de la persona que se desea agregar a la lista negra.
        /// - **`idEstablishment` (int, requerido):** El ID del establecimiento al que se aplicará la restricción.
        /// - **`reason` (string, opcional):** La razón por la cual la persona es añadida a la lista negra.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto Blacklist creado (incluyendo su Id, IdPerson, IdEstablishment, Reason), y 'Message' es "Persona añadida a la lista negra.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos." si el DTO no es válido. 'Data' puede contener una lista de errores de validación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para este establecimiento." si el usuario no tiene los permisos necesarios para el establecimiento especificado.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Persona no encontrada." si el IdPerson no existe, o "Establecimiento no encontrado." si el IdEstablishment no existe.</response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "La persona ya se encuentra en la lista negra de este establecimiento." si ya existe una entrada para esa persona en ese establecimiento.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error del servidor." si ocurre un error interno no controlado.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] BlacklistDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, "Datos inválidos."));

            if (!_userContext.CanAccessOwnEstablishment(dto.IdEstablishment))
                return StatusCode(403, new ResponseDto(403, message: "No tienes permiso para este establecimiento."));

            var response = await _blacklistService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Elimina un registro de la lista negra de un establecimiento específico utilizando el ID de la persona y el ID del establecimiento.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// El usuario debe tener permisos para eliminar el registro del establecimiento especificado.
        ///
        /// A diferencia de la eliminación por ID de registro de blacklist, este método permite identificar y eliminar una entrada
        /// específica basándose en la **combinación única de la persona y el establecimiento**. La eliminación es **física**.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para DeleteBlacklistByPersonIdDto:**
        /// ```json
        /// {
        ///   "idPerson": 123,
        ///   "idEstablishment": 1
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los identificadores de la persona y el establecimiento.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`idPerson` (int, requerido):** El ID de la persona cuyo registro se desea eliminar de la lista negra.
        /// - **`idEstablishment` (int, requerido):** El ID del establecimiento del cual se eliminará el registro de la lista negra.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Persona eliminada de la lista negra." si la operación fue exitosa.</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos." si el DTO no es válido. 'Data' puede contener una lista de errores de validación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para eliminar en este establecimiento." si el usuario no tiene los permisos necesarios sobre el establecimiento.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Persona no encontrada." si el `IdPerson` no existe; "Establecimiento no encontrado." si el `IdEstablishment` no existe; o "Campo de lista negra no encontrada." si no existe una entrada en la lista negra para la combinación de persona y establecimiento proporcionada.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error del servidor." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("deleteByPersonId")]
        public async Task<ActionResult<ResponseDto>> DeleteUserFromBlacklist([FromBody] DeleteBlacklistByPersonIdDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));

            var response = await _blacklistService.DeleteByPersonAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene una lista de todos los registros de la lista negra, filtrados por los permisos del usuario.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// Si el usuario es **SUPERADMIN**, se devolverán todos los registros de la lista negra de todos los establecimientos.
        /// Si el usuario es **ADMIN**, solo se devolverán los registros de la lista negra asociados a su establecimiento.
        /// Cada registro incluye los detalles de la persona (`Person`) y el establecimiento (`Establishment`) asociados.
        /// </remarks>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos <see cref="Blacklist"/>, cada uno con los detalles de la persona y el establecimiento.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "Establecimiento no asociado." si el usuario no es SUPERADMIN y no tiene un establecimiento asociado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error del servidor." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _blacklistService.GetAllAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene un registro específico de la lista negra por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Los usuarios solo pueden acceder a los registros de la lista negra de su propio establecimiento, a menos que tengan el rol SUPERADMIN.
        /// La respuesta incluirá los detalles completos del registro, incluyendo la información de la persona (`Person`) y el establecimiento (`Establishment`) asociados.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del registro de la lista negra a recuperar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="Blacklist"/> completo con los detalles de la persona y el establecimiento, y 'Message' es "Entrada obtenida.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para ver esta entrada." si el usuario no tiene los permisos para acceder al establecimiento del registro.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Entrada no encontrada." si el registro con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error del servidor." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _blacklistService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Actualiza la información de un registro existente en la lista negra por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// El usuario debe tener permisos para modificar tanto el establecimiento original del registro como el nuevo establecimiento, si se realiza un cambio.
        ///
        /// Permite modificar la persona asociada, el establecimiento al que pertenece el registro y la razón de la inclusión en la lista negra.
        /// No se permite duplicar una entrada existente, es decir, una persona no puede estar dos veces en la lista negra del mismo establecimiento.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para BlacklistDto:**
        /// ```json
        /// {
        ///   "idPerson": 123,
        ///   "idEstablishment": 1,
        ///   "reason": "Acceso denegado por incumplimiento de normas de seguridad"
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del registro de la lista negra a actualizar.</param>
        /// <param name="dto">
        /// **Objeto DTO con los datos actualizados del registro de la lista negra.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`idPerson` (int, requerido):** El ID de la persona para este registro de lista negra.
        /// - **`idEstablishment` (int, requerido):** El ID del establecimiento al que pertenece este registro.
        /// - **`reason` (string, opcional):** La razón actualizada para la inclusión en la lista negra.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="Blacklist"/> actualizado y 'Message' es "Entrada actualizada correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos." si el DTO no es válido. 'Data' puede contener una lista de errores de validación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para modificar esta entrada." si el usuario no tiene los permisos necesarios sobre el establecimiento original o el nuevo.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Entrada no encontrada." si el registro con el ID proporcionado no existe; "Persona no encontrada." si el `IdPerson` no existe; o "Establecimiento no encontrado." si el `IdEstablishment` no existe.</response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "Ya existe una entrada similar en la lista negra." si los datos proporcionados (persona y establecimiento) ya existen en otro registro de la lista negra.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error del servidor." si ocurre un error interno no controlado.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] BlacklistDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));

            var response = await _blacklistService.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Elimina un registro de la lista negra por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// El usuario debe tener permisos para eliminar el registro del establecimiento asociado.
        ///
        /// La eliminación de un registro de lista negra es **física**, lo que significa que el registro será borrado completamente
        /// de la base de datos.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del registro de la lista negra a eliminar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Entrada eliminada." si la operación fue exitosa.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para eliminar esta entrada." si el usuario no tiene los permisos para acceder al establecimiento del registro.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Entrada no encontrada." si el registro con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error del servidor." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _blacklistService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}