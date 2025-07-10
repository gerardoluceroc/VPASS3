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
    public class CommonAreaReservationController : ControllerBase
    {
        private readonly ICommonAreaReservationService _svc;
        private readonly IUserContextService _userCtx;

        public CommonAreaReservationController(ICommonAreaReservationService svc, IUserContextService userCtx)
        {
            _svc = svc;
            _userCtx = userCtx;
        }

        /// <summary>
        /// Crea una nueva reserva para un área común específica.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// Para que una reserva sea exitosa:
        /// - El área común debe existir y estar habilitada para reservas (`Mode` debe incluir `Reservable`).
        /// - La persona que realiza la reserva (`IdPersonReservedBy`) debe existir.
        /// - Si `ReservationStart` no se especifica o es un valor por defecto, se usará la **hora actual de Santiago de Chile**.
        /// - La fecha y hora de fin de la reserva (`ReservationEnd`, calculada a partir de `ReservationStart` y `ReservationTime`) no puede ser en el pasado.
        /// - No debe haber conflictos de horario con otras reservas existentes para la misma área común (las reservas no pueden solaparse).
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para CreateCommonAreaReservationDto:**
        /// ```json
        /// {
        ///   "idCommonArea": 101,
        ///   "idPersonReservedBy": 50,
        ///   "reservationStart": "2025-07-15T10:00:00", // Opcional: Si no se envía, se usa la hora actual de Santiago
        ///   "reservationTime": "02:00:00", // Opcional: Duración de la reserva en formato HH:mm:ss
        ///   "guestsNumber": 4 // Opcional: Número de invitados. Por defecto es 0 si no se envía.
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para la creación de la reserva.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`IdCommonArea` (int, requerido):** El ID del área común que se desea reservar.
        /// - **`IdPersonReservedBy` (int, requerido):** El ID de la persona que está realizando la reserva.
        /// - **`ReservationStart` (DateTime?, opcional):** La fecha y hora de inicio de la reserva. Si se omite o es nulo, se utilizará la **hora actual del servidor (Zona Horaria de Santiago de Chile)**.
        /// - **`ReservationTime` (TimeSpan?, opcional):** La duración de la reserva en formato HH:mm:ss. Si no se especifica, se asume una duración de cero (esto afecta la validación de tiempo pasado).
        /// - **`GuestsNumber` (int?, opcional):** El número de invitados que acompañarán a la persona que reserva. Si no se proporciona, se asume 0.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="CommonAreaReservation"/> creado y 'Message' es "Reserva creada.".</response>
        /// <response code="400">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - Los datos del DTO no son válidos ("Datos inválidos. Revisa los campos.").
        /// - El área común no está habilitada para reservas ("El área no está habilitada para reservas.").
        /// - La reserva (considerando `ReservationStart` y `ReservationTime`) termina en el pasado ("La reserva no puede estar en el pasado.").
        /// - Ya existe otra reserva en el mismo horario para esta área común ("Ya existe una reserva en este horario para esta área.").
        /// </response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No puedes reservar este espacio." si el usuario no tiene los permisos para acceder al área común.</response>
        /// <response code="404">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - El área común con el `IdCommonArea` no existe ("Área común no existe.").
        /// - La persona que reserva con el `IdPersonReservedBy` no se encuentra ("Persona que reserva no encontrada.").
        /// </response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al crear la reserva." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateCommonAreaReservationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Revisa los campos."));

            var resp = await _svc.CreateAsync(dto);
            return StatusCode(resp.StatusCode, resp);
        }

        /// <summary>
        /// Obtiene una lista de todas las reservas de áreas comunes, filtradas según los permisos del usuario.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Si el usuario es **SUPERADMIN**, se devolverán todas las reservas de todos los establecimientos.
        /// - Si el usuario es **ADMIN**, solo se devolverán las reservas que pertenezcan al establecimiento asociado a su cuenta.
        ///
        /// Cada reserva incluirá los detalles de la persona que la realizó (`ReservedBy`) para una mejor visibilidad.
        /// </remarks>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos <see cref="CommonAreaReservation"/>, cada uno con los detalles de la reserva y la persona que reservó. El 'Message' es "Reservas obtenidas correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes un establecimiento asociado." si el usuario no es SUPERADMIN y no tiene un ID de establecimiento asignado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al obtener reservas." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var resp = await _svc.GetAllAsync();
            return StatusCode(resp.StatusCode, resp);
        }

        /// <summary>
        /// Obtiene los detalles de una reserva de área común específica por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Los usuarios con rol **SUPERADMIN** pueden acceder a cualquier reserva por su ID.
        /// - Los usuarios con rol **ADMIN** solo pueden ver las reservas que pertenezcan al establecimiento asociado a su cuenta.
        ///
        /// La respuesta incluirá los detalles completos de la reserva, incluyendo la información de la persona que realizó la reserva (`ReservedBy`).
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) de la reserva de área común a recuperar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="CommonAreaReservation"/> completo con los detalles de la persona que reservó. El 'Message' es "Reserva obtenida correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No puedes ver esta reserva." si el usuario no tiene los permisos para acceder al establecimiento del área común asociada a la reserva, o si no tiene un establecimiento asociado.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Reserva no encontrada." si la reserva con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al obtener la reserva." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var resp = await _svc.GetByIdAsync(id);
            return StatusCode(resp.StatusCode, resp);
        }

        /// <summary>
        /// Actualiza los detalles de una reserva de área común existente por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Solo los usuarios con permisos para administrar el establecimiento al que pertenece el área común de la reserva pueden realizar actualizaciones.
        ///
        /// Puedes modificar la fecha y hora de inicio (`ReservationStart`), la duración (`ReservationTime`) y el número de invitados (`GuestsNumber`).
        /// - Los campos en el DTO son opcionales; si no se proporcionan, se mantendrá el valor actual de la reserva.
        /// - La **fecha y hora de fin de la reserva** (calculada) no puede ser en el pasado.
        /// - Se valida que el área común siga estando habilitada para reservas (`Mode` debe incluir `Reservable`).
        /// - No se permite que la reserva actualizada se solape con otras reservas existentes para la misma área común.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para UpdateCommonAreaReservationDto:**
        /// ```json
        /// {
        ///   "reservationStart": "2025-07-15T14:00:00", // Cambiar la hora de inicio
        ///   "reservationTime": "01:30:00", // Cambiar la duración a 1 hora y 30 minutos
        ///   "guestsNumber": 6 // Aumentar el número de invitados
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) de la reserva de área común a actualizar.</param>
        /// <param name="dto">
        /// **Objeto DTO con los datos que se desean actualizar en la reserva.**
        /// Se espera un objeto JSON con las siguientes propiedades opcionales:
        /// - **`ReservationStart` (DateTime?, opcional):** La nueva fecha y hora de inicio de la reserva.
        /// - **`ReservationTime` (TimeSpan?, opcional):** La nueva duración de la reserva en formato HH:mm:ss.
        /// - **`GuestsNumber` (int?, opcional):** El nuevo número de invitados para la reserva. Si se omite, se mantiene el número actual.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="CommonAreaReservation"/> actualizado y 'Message' es "Reserva actualizada.".</response>
        /// <response code="400">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - Los datos del DTO no son válidos ("Datos inválidos. Revisa los campos.").
        /// - El área común asociada a la reserva no está habilitada para reservas ("El área no está habilitada para reservas.").
        /// - La reserva, con los nuevos datos, termina en el pasado ("La reserva no puede estar en el pasado.").
        /// - La reserva actualizada se solapa con otra reserva existente para la misma área ("Ya existe una reserva en este horario para esta área.").
        /// </response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No puedes editar esta reserva." si el usuario no tiene los permisos para acceder al establecimiento del área común asociada a la reserva.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Reserva no encontrada." si la reserva con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al actualizar la reserva." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] UpdateCommonAreaReservationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Revisa los campos."));

            var resp = await _svc.UpdateAsync(id, dto);
            return StatusCode(resp.StatusCode, resp);
        }

        /// <summary>
        /// Elimina una reserva de área común de forma física por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Solo los usuarios con permisos para administrar el establecimiento al que pertenece el área común asociada a la reserva pueden eliminarla.
        ///
        /// La eliminación es **física**, lo que significa que el registro de la reserva será removido permanentemente de la base de datos.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) de la reserva de área común a eliminar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Reserva eliminada." si la operación fue exitosa.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No puedes eliminar esta reserva." si el usuario no tiene los permisos para acceder al establecimiento del área común asociada a la reserva.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Reserva no encontrada." si la reserva con el ID proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al eliminar la reserva." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var resp = await _svc.DeleteAsync(id);
            return StatusCode(resp.StatusCode, resp);
        }
    }
}