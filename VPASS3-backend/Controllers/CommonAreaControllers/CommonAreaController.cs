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
    public class CommonAreaController : ControllerBase
    {
        private readonly ICommonAreaService _service;
        private readonly IUserContextService _userContext;

        public CommonAreaController(ICommonAreaService service, IUserContextService userContext)
        {
            _service = service;
            _userContext = userContext;
        }

        /// <summary>
        /// Crea una nueva área común para un establecimiento específico.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Solo los usuarios con permisos para administrar el establecimiento pueden crear áreas comunes en él.
        /// El nombre del área común debe ser único dentro del establecimiento.
        ///
        /// La propiedad `Mode` define el tipo de uso del área y puede ser una combinación de:
        /// - `None` (0): No usable ni reservable.
        /// - `Usable` (1): Puede ser usada por múltiples personas hasta su capacidad máxima (ej. gimnasio, piscina). Si es `Usable`, `MaxCapacity` es relevante.
        /// - `Reservable` (2): Puede ser reservada exclusivamente por una persona/grupo (ej. centro de eventos, quincho).
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para CreateCommonAreaDto (Área Usable):**
        /// ```json
        /// {
        ///   "name": "Gimnasio Principal",
        ///   "idEstablishment": 1,
        ///   "mode": 1, // Usable
        ///   "maxCapacity": 20
        /// }
        /// ```
        /// **Ejemplo de cuerpo de petición (Request Body) para CreateCommonAreaDto (Área Reservable):**
        /// ```json
        /// {
        ///   "name": "Sala de Eventos",
        ///   "idEstablishment": 1,
        ///   "mode": 2, // Reservable
        ///   "maxCapacity": null // Opcional, pero no relevante para Reservable
        /// }
        /// ```
        /// **Ejemplo de cuerpo de petición (Request Body) para CreateCommonAreaDto (Área Usable y Reservable):**
        /// ```json
        /// {
        ///   "name": "Sala de Reuniones Grande",
        ///   "idEstablishment": 1,
        ///   "mode": 3, // Usable | Reservable (1 + 2)
        ///   "maxCapacity": 15
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para la creación del área común.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`name` (string, requerido):** El nombre del área común (ej., "Piscina", "Sala de Reuniones").
        /// - **`idEstablishment` (int, requerido):** El ID del establecimiento al que pertenecerá esta área común.
        /// - **`mode` (CommonAreaMode, requerido):** El modo de uso del área (Usable, Reservable, o una combinación). Valores posibles: 0 (None), 1 (Usable), 2 (Reservable), 3 (Usable y Reservable).
        /// - **`maxCapacity` (int?, opcional):** La capacidad máxima del área. Solo es relevante si `Mode` incluye `Usable`. Puede ser nulo si no aplica.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto CommonArea creado (incluyendo su Id generado automáticamente, Name, IdEstablishment, Mode, MaxCapacity, Status por defecto), y 'Message' es "Área común creada correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos." si el DTO no es válido. 'Data' puede contener una lista de errores de validación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para crear aquí." si el usuario no tiene los permisos para el establecimiento especificado.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Establecimiento no encontrado." si el IdEstablishment proporcionado no existe.</response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "Ya existe un área común con este nombre." si ya hay un área común con el mismo nombre en el mismo establecimiento.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al crear el área común." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateCommonAreaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));
            var res = await _service.CreateAsync(dto);
            return StatusCode(res.StatusCode, res);
        }

        /// <summary>
        /// Obtiene una lista de todas las áreas comunes, filtradas por los permisos del usuario.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// Si el usuario es **SUPERADMIN**, se devolverán todas las áreas comunes de todos los establecimientos.
        /// Si el usuario es **ADMIN**, solo se devolverán las áreas comunes asociadas a su establecimiento.
        /// Cada área común en la respuesta incluirá sus detalles (ID, nombre, establecimiento, modo, capacidad máxima, estado),
        /// así como sus **reservas** (`Reservations`) y **registros de uso** (`Usages`), incluyendo la información de las personas (`Person`) asociadas a cada uno.
        /// </remarks>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos <see cref="CommonArea"/> completos, con sus reservas y usos incluidos. El 'Message' es "Áreas comunes obtenidas correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes un establecimiento asociado." si el usuario no es SUPERADMIN y no tiene un ID de establecimiento asignado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al obtener áreas comunes." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var res = await _service.GetAllAsync();
            return StatusCode(res.StatusCode, res);
        }

        /// <summary>
        /// Obtiene los detalles de un área común específica por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// Los usuarios solo pueden acceder a las áreas comunes que pertenecen a su propio establecimiento, a menos que tengan el rol SUPERADMIN.
        /// La respuesta incluirá los detalles completos del área común (ID, nombre, establecimiento, modo, capacidad máxima, estado),
        /// así como sus **reservas** (`Reservations`) y **registros de uso** (`Usages`), incluyendo la información de las personas (`Person`) asociadas a cada uno.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del área común a recuperar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="CommonArea"/> completo con sus reservas y usos incluidos. El 'Message' es "Área común obtenida correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para este recurso." si el usuario no tiene los permisos para acceder al establecimiento del área común.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Área común no encontrada." si el área común con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al obtener el área común." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var res = await _service.GetByIdAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        /// <summary>
        /// Actualiza la información de un área común existente por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Solo los usuarios con permisos para administrar el establecimiento al que pertenece el área común pueden realizar actualizaciones.
        ///
        /// Permite modificar el nombre, el modo de uso (`Mode`), la capacidad máxima (`MaxCapacity` si es `Usable`), y el estado (`Status`) del área común.
        /// - Si el `Mode` cambia y ya no incluye `Usable`, la `MaxCapacity` se establecerá automáticamente en `null`.
        /// - El campo `Status` es opcional en el DTO; si se proporciona, se actualizará el estado del área.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para UpdateCommonAreaDto:**
        /// ```json
        /// {
        ///   "name": "Gimnasio Renovado",
        ///   "idEstablishment": 1, // Este campo es solo informativo en el DTO de actualización
        ///   "mode": 3, // Usable | Reservable (si el gimnasio ahora también se puede reservar)
        ///   "maxCapacity": 25, // Aumento de capacidad si el modo es usable
        ///   "status": 0 // Disponible (Available)
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del área común a actualizar.</param>
        /// <param name="dto">
        /// **Objeto DTO con los datos actualizados del área común.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`name` (string, requerido):** El nuevo nombre del área común.
        /// - **`idEstablishment` (int, requerido):** El ID del establecimiento al que pertenece esta área común. (Nota: Aunque requerido en el DTO, el establecimiento del área se determina por su ID y no se puede cambiar directamente a través de esta operación; se utiliza principalmente para validación de permisos).
        /// - **`mode` (CommonAreaMode, requerido):** El nuevo modo de uso del área (Usable, Reservable, o una combinación). Valores posibles: 0 (None), 1 (Usable), 2 (Reservable), 3 (Usable y Reservable).
        /// - **`maxCapacity` (int?, opcional):** La nueva capacidad máxima del área. Solo es relevante si `Mode` incluye `Usable`. Puede ser nulo si no aplica o si el modo ya no es usable.
        /// - **`status` (CommonAreaStatus?, opcional):** El nuevo estado del área (Available, Maintenance, TemporarilyClosed). Si se omite, el estado actual se mantiene.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="CommonArea"/> actualizado y 'Message' es "Área común actualizada correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos." si el DTO no es válido. 'Data' puede contener una lista de errores de validación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para actualizar esta área." si el usuario no tiene los permisos para acceder al establecimiento del área común.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Área común no encontrada." si el área común con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al actualizar el área común." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] UpdateCommonAreaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));
            var res = await _service.UpdateAsync(id, dto);
            return StatusCode(res.StatusCode, res);
        }

        /// <summary>
        /// Elimina un área común de forma física por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Solo los usuarios con permisos para administrar el establecimiento al que pertenece el área común pueden eliminarla.
        ///
        /// La eliminación es **física**, lo que significa que el registro del área común será removido permanentemente de la base de datos.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del área común a eliminar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Área común eliminada correctamente." si la operación fue exitosa.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para eliminar esta área." si el usuario no tiene los permisos para acceder al establecimiento del área común.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Área común no encontrada." si el área común con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al eliminar el área común." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var res = await _service.DeleteAsync(id);
            return StatusCode(res.StatusCode, res);
        }
    }
}