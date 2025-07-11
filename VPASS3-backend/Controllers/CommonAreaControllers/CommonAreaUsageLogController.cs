using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces.CommonAreaInterfaces;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models.CommonAreas;

namespace VPASS3_backend.Controllers.CommonAreaControllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommonAreaUsageLogController : ControllerBase
    {
        private readonly ICommonAreaUsageLogService _service;
        private readonly IUserContextService _userCtx;

        public CommonAreaUsageLogController(ICommonAreaUsageLogService service, IUserContextService userCtx)
        {
            _service = service;
            _userCtx = userCtx;
        }

        /// <summary>
        /// Registra un nuevo uso en un área común específica.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// Para que el registro de uso sea exitoso:
        /// - El área común debe existir.
        /// - El área común debe estar habilitada para uso (`Mode` debe incluir `Usable`).
        /// - El área común debe estar disponible (`Status` debe ser `Available`).
        /// - La persona que registra el uso (`IdPerson`) debe existir.
        /// - Si `StartTime` no se especifica o es un valor por defecto, se usará la **hora actual de Santiago de Chile**.
        ///
        /// **Importante:** Actualmente, este registro solo graba el uso. No se realizan validaciones de capacidad máxima
        /// ni de concurrencia de usos en el momento de la creación, esto se contempla para futuras implementaciones.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para CreateUsageLogDto:**
        /// ```json
        /// {
        ///   "idCommonArea": 102,
        ///   "idPerson": 51,
        ///   "startTime": "2025-07-11T11:00:00", // Opcional: Si no se envía, se usa la hora actual
        ///   "usageTime": "01:00:00", // Opcional: Duración del uso en formato HH:mm:ss
        ///   "guestsNumber": 2 // Opcional: Número de invitados
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para la creación del registro de uso.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`IdCommonArea` (int, requerido):** El ID del área común donde se registra el uso.
        /// - **`IdPerson` (int, requerido):** El ID de la persona que está registrando el uso del área.
        /// - **`StartTime` (DateTime?, opcional):** La fecha y hora de inicio del uso. Si se omite o es nulo, se utilizará la **hora actual del servidor (Zona Horaria de Santiago de Chile)**.
        /// - **`UsageTime` (TimeSpan?, opcional):** La duración esperada del uso en formato HH:mm:ss. Puede ser nulo si el uso es de duración indefinida o se registra solo el inicio.
        /// - **`GuestsNumber` (int?, opcional):** El número de invitados que acompañan a la persona. Si no se proporciona, se asume 0.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="CommonAreaUsageLog"/> creado y 'Message' es "Uso registrado correctamente.".</response>
        /// <response code="400">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - Los datos del DTO no son válidos ("Datos inválidos.").
        /// - El área común no está habilitada para usos ("El área no permite uso.").
        /// - El área común no está disponible ("Área no disponible actualmente.").
        /// </response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "Sin permiso para usar este espacio." si el usuario no tiene los permisos para acceder al área común.</response>
        /// <response code="404">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - El área común con el `IdCommonArea` no existe ("Área común no existe.").
        /// - La persona con el `IdPerson` no se encuentra ("Persona no encontrada.").
        /// </response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al registrar el uso." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateUsageLogDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));

            var resp = await _service.CreateUsageAsync(dto);
            return StatusCode(resp.StatusCode, resp);
        }

        /// <summary>
        /// Obtiene una lista de todos los registros de uso de áreas comunes, filtrados según los permisos del usuario.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Si el usuario es **SUPERADMIN**, se devolverán todos los registros de uso de todos los establecimientos.
        /// - Si el usuario es **ADMIN**, solo se devolverán los registros de uso que pertenezcan a áreas comunes de su propio establecimiento.
        ///
        /// Cada registro de uso en la respuesta incluirá los detalles de la persona que realizó el uso (`Person`) para una mejor visibilidad.
        /// </remarks>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos <see cref="CommonAreaUsageLog"/>, cada uno con los detalles del uso y la persona asociada. El 'Message' es "Usos obtenidos correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes un establecimiento asociado." si el usuario no es SUPERADMIN y no tiene un ID de establecimiento asignado (aunque el servicio lo manejaría internamente, es una condición a considerar).</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al obtener los registros de uso." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var resp = await _service.GetAllAsync();
            return StatusCode(resp.StatusCode, resp);
        }

        /// <summary>
        /// Obtiene los detalles de un registro de uso de área común específico por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Los usuarios con rol **SUPERADMIN** pueden acceder a cualquier registro de uso por su ID.
        /// - Los usuarios con rol **ADMIN** solo pueden ver los registros de uso que pertenezcan a áreas comunes de su propio establecimiento.
        ///
        /// La respuesta incluirá los detalles completos del registro de uso, incluyendo la información de la persona que realizó el uso (`Person`).
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del registro de uso de área común a recuperar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="CommonAreaUsageLog"/> completo con los detalles de la persona asociada. El 'Message' es "Uso obtenido correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "Sin permiso para ver este uso." si el usuario no tiene los permisos para acceder al establecimiento del área común asociada al registro de uso.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Uso no encontrado." si el registro de uso con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al obtener el uso." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var resp = await _service.GetByIdAsync(id);
            return StatusCode(resp.StatusCode, resp);
        }

        /// <summary>
        /// Actualiza los detalles de un registro de uso de área común existente por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Solo los usuarios con permisos para administrar el establecimiento al que pertenece el área común asociada al uso pueden realizar actualizaciones.
        ///
        /// Se pueden modificar la fecha y hora de inicio (`StartTime`), la duración (`UsageTime`) y el número de invitados (`GuestsNumber`).
        /// - Si `StartTime` no se especifica o es un valor por defecto en el DTO, se usará la **hora actual de Santiago de Chile** como nuevo `StartTime`.
        /// - El área común asociada al registro de uso debe estar habilitada para usos (`Mode` debe incluir `Usable`) y disponible (`Status` debe ser `Available`) para poder actualizar el registro.
        ///
        /// **Importante:** Al igual que en la creación, actualmente no se realizan validaciones de capacidad máxima al actualizar el uso.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para UpdateUsageLogDto:**
        /// ```json
        /// {
        ///   "startTime": "2025-07-11T12:00:00", // Cambiar la hora de inicio del uso
        ///   "usageTime": "01:45:00", // Cambiar la duración del uso
        ///   "guestsNumber": 3 // Actualizar el número de invitados
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del registro de uso a actualizar.</param>
        /// <param name="dto">
        /// **Objeto DTO con los datos que se desean actualizar en el registro de uso.**
        /// Se espera un objeto JSON con las siguientes propiedades opcionales:
        /// - **`StartTime` (DateTime?, opcional):** La nueva fecha y hora de inicio del uso.
        /// - **`UsageTime` (TimeSpan?, opcional):** La nueva duración del uso en formato HH:mm:ss.
        /// - **`GuestsNumber` (int?, opcional):** El nuevo número de invitados. Si se omite, se mantiene el valor actual.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="CommonAreaUsageLog"/> actualizado y 'Message' es "Uso actualizado correctamente.".</response>
        /// <response code="400">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - Los datos del DTO no son válidos ("Datos inválidos.").
        /// - El área común asociada al registro no está habilitada para usos ("El área no está habilitada para usos.").
        /// - El área común asociada al registro no está disponible ("El área no está disponible.").
        /// </response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para editar este uso." si el usuario no tiene los permisos para acceder al establecimiento del área común asociada al uso.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Registro de uso no encontrado." si el registro con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al actualizar el uso." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] UpdateUsageLogDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));

            var resp = await _service.UpdateAsync(id, dto);
            return StatusCode(resp.StatusCode, resp);
        }

        /// <summary>
        /// Elimina un registro de uso de área común de forma física por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Solo los usuarios con permisos para administrar el establecimiento al que pertenece el área común asociada al uso pueden eliminarlo.
        ///
        /// La eliminación es **física**, lo que significa que el registro de uso será removido permanentemente de la base de datos.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del registro de uso a eliminar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Uso eliminado correctamente." si la operación fue exitosa.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "Sin permiso para eliminar este uso." si el usuario no tiene los permisos para acceder al establecimiento del área común asociada al uso.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Uso no encontrado." si el registro de uso con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al eliminar el uso." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var resp = await _service.DeleteUsageAsync(id);
            return StatusCode(resp.StatusCode, resp);
        }
    }
}