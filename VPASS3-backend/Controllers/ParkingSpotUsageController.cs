using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.ParkingSpotUsageLogs;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParkingSpotUsageLogController : ControllerBase
    {
        private readonly IParkingSpotUsageLogService _service;

        public ParkingSpotUsageLogController(IParkingSpotUsageLogService service)
        {
            _service = service;
        }

        /// <summary>
        /// Registra un nuevo uso de estacionamiento o completa un registro de uso existente,
        /// basándose en una visita de entrada o salida que incluye un vehículo.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// El usuario debe tener permisos para registrar usos en el establecimiento asociado al estacionamiento.
        ///
        /// La lógica de este endpoint es la siguiente:
        /// - Si la `Visit` proporcionada en el DTO es de tipo **"entrada"** y `VehicleIncluded` es `true`:
        ///   Se crea un nuevo registro de uso (`ParkingSpotUsageLog`) con la hora de entrada de la visita y el tiempo autorizado.
        /// - Si la `Visit` proporcionada en el DTO es de tipo **"salida"** y `VehicleIncluded` es `true`:
        ///   Se busca el último registro de uso de estacionamiento **abierto** (sin `IdExitVisit` ni `EndTime`) para la persona asociada a la visita.
        ///   Si se encuentra, se actualiza este registro con la hora de salida de la visita y se calcula el tiempo de uso (`UsageTime`).
        ///
        /// **Importante:** Este endpoint actúa como un "toggle" inteligente que crea un log de entrada o cierra uno de salida
        /// dependiendo de la dirección de la visita.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para ParkingSpotUsageLogDto:**
        /// ```json
        /// {
        ///   "idVisit": 456 // ID de la visita de entrada o salida que incluye un vehículo
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con el ID de la visita asociada al uso del estacionamiento.**
        /// Se espera un objeto JSON con la siguiente propiedad:
        /// - **`IdVisit` (int, requerido):** El ID de la visita (de entrada o salida) que desencadena el registro de uso del estacionamiento. Esta visita debe tener `VehicleIncluded` en `true`.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el nuevo objeto <see cref="ParkingSpotUsageLog"/> creado y 'Message' es "Registro de entrada al estacionamiento creado." (para visitas de entrada).</response>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="ParkingSpotUsageLog"/> actualizado y 'Message' es "Registro de salida actualizado correctamente." (para visitas de salida).</response>
        /// <response code="400">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - Los datos del DTO no son válidos ("Datos inválidos. Verifica los campos.").
        /// - La dirección de la visita no es "entrada" ni "salida" ("Dirección de la visita inválida. Debe ser entrada o salida.").
        /// </response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para registrar uso en este estacionamiento." si el usuario no tiene los permisos para el establecimiento del estacionamiento.</response>
        /// <response code="404">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - La visita con el `IdVisit` no existe ("Visita no encontrada.").
        /// - El estacionamiento asociado a la visita no existe ("Estacionamiento no encontrado.").
        /// - La persona asociada a la visita no existe ("Visitante no encontrado.").
        /// - No se encontró un registro de uso de estacionamiento abierto para la persona en caso de una visita de salida ("No se encontró un uso de estacionamiento abierto para este visitante.").
        /// </response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al registrar el uso del estacionamiento." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] ParkingSpotUsageLogDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos."));

            var response = await _service.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene una lista de todos los registros de uso de un estacionamiento, filtrados según los permisos del usuario.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Si el usuario es **SUPERADMIN**, se devolverán todos los registros de uso de estacionamiento de todos los establecimientos.
        /// - Si el usuario es **ADMIN**, solo se devolverán los registros de uso de estacionamiento que pertenezcan al establecimiento asociado a su cuenta.
        ///
        /// Cada registro de uso de estacionamiento incluirá detalles completos de la **visita de entrada** asociada (`EntryVisit`),
        /// incluyendo información sobre la zona (`Zone`), apartamento (`Apartment`), dirección (`Direction`), tipo de visita (`VisitType`),
        /// el estacionamiento utilizado (`ParkingSpot`) y la persona (`Person`) involucrada.
        /// </remarks>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos <see cref="ParkingSpotUsageLog"/> con sus relaciones cargadas. El 'Message' es "Registros de uso de estacionamiento obtenidos correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes un establecimiento asociado." si el usuario no es SUPERADMIN y no tiene un ID de establecimiento asignado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener los registros." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _service.GetAllAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene los detalles de un registro de uso de estacionamiento específico por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Los usuarios con rol **SUPERADMIN** pueden acceder a cualquier registro de uso de estacionamiento por su ID.
        /// - Los usuarios con rol **ADMIN** solo pueden ver los registros de uso que pertenezcan a estacionamientos de su propio establecimiento.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del registro de uso de estacionamiento a recuperar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="ParkingSpotUsageLog"/> completo. El 'Message' es "Registro obtenido correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para acceder a este registro." si el usuario no tiene los permisos para el establecimiento del estacionamiento asociado al registro de uso.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Registro no encontrado." si el ID proporcionado no corresponde a un registro de uso de estacionamiento existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener el registro." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Elimina un registro de uso de un estacionamiento de forma física por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Los usuarios con rol **SUPERADMIN** pueden eliminar cualquier registro de uso de estacionamiento por su ID.
        /// - Los usuarios con rol **ADMIN** solo pueden eliminar los registros de uso que pertenezcan a estacionamientos de su propio establecimiento.
        ///
        /// La eliminación es **física**, lo que significa que el registro será removido permanentemente de la base de datos.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del registro de uso de estacionamiento a eliminar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Registro eliminado correctamente." si la operación fue exitosa.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para eliminar este registro." si el usuario no tiene los permisos para el establecimiento del estacionamiento asociado al registro de uso.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Registro no encontrado." si el ID proporcionado no corresponde a un registro de uso de estacionamiento existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al eliminar el registro." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _service.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
