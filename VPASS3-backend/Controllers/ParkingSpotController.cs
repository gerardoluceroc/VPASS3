using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.ParkingSpots;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using VPASS3_backend.Filters;
using VPASS3_backend.Models;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParkingSpotController : ControllerBase
    {
        private readonly IParkingSpotService _service;

        public ParkingSpotController(IParkingSpotService service)
        {
            _service = service;
        }

        /// <summary>
        /// Crea un nuevo espacio de estacionamiento asociado a un establecimiento.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Solo los usuarios con permisos para administrar el establecimiento pueden crear estacionamientos en él.
        ///
        /// Al crear un estacionamiento, su estado `IsAvailable` se establece por defecto en `true`.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para ParkingSpotDto:**
        /// ```json
        /// {
        ///   "name": "E-101",
        ///   "idEstablishment": 1
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para la creación del espacio de estacionamiento.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`name` (string, opcional):** El nombre del estacionamiento (ej., "E-101").
        /// - **`idEstablishment` (int, requerido):** El ID del establecimiento al que pertenecerá este estacionamiento.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="ParkingSpot"/> creado (incluyendo su Id generado automáticamente, Name, IdEstablishment, IsAvailable por defecto), y 'Message' es "Estacionamiento creado correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos." si el DTO no es válido. 'Data' puede contener una lista de errores de validación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para crear un estacionamiento en este establecimiento." si el usuario no tiene los permisos para el establecimiento especificado.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Establecimiento no encontrado." si el IdEstablishment proporcionado no existe.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al crear el estacionamiento." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] ParkingSpotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, "Datos inválidos."));

            var response = await _service.CreateParkingSpotAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene una lista de todos los espacios de estacionamiento, filtrados según los permisos del usuario.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Si el usuario es **SUPERADMIN**, se devolverán todos los estacionamientos de todos los establecimientos.
        /// - Si el usuario es **ADMIN**, solo se devolverán los estacionamientos que pertenezcan al establecimiento asociado a su cuenta.
        ///
        /// Cada espacio de estacionamiento incluirá los detalles de su establecimiento asociado (`Establishment`) para una mejor visibilidad.
        /// </remarks>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos <see cref="ParkingSpot"/>, cada uno con los detalles del estacionamiento y su establecimiento. El 'Message' es "Estacionamientos obtenidos correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes un establecimiento asociado." si el usuario no es SUPERADMIN y no tiene un ID de establecimiento asignado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener los estacionamientos." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _service.GetAllParkingSpotsAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene los detalles de un espacio de estacionamiento específico por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Los usuarios con rol **SUPERADMIN** pueden acceder a cualquier estacionamiento por su ID.
        /// - Los usuarios con rol **ADMIN** solo pueden ver los estacionamientos que pertenezcan al establecimiento asociado a su cuenta.
        ///
        /// La respuesta incluirá los detalles completos del estacionamiento, incluyendo la información de su establecimiento asociado (`Establishment`).
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del espacio de estacionamiento a recuperar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="ParkingSpot"/> completo con los detalles del establecimiento asociado. El 'Message' es "Estacionamiento obtenido correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para acceder a este estacionamiento." si el usuario no tiene los permisos para el establecimiento del estacionamiento.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Estacionamiento no encontrado." si el ID proporcionado no corresponde a un estacionamiento existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener el estacionamiento." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _service.GetParkingSpotByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Actualiza la información de un espacio de estacionamiento existente por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Solo los usuarios con permisos para administrar el establecimiento al que pertenece el estacionamiento pueden editarlo o moverlo.
        ///
        /// Permite modificar el nombre (`Name`), el estado de disponibilidad (`IsAvailable`) y el establecimiento al que pertenece (`IdEstablishment`).
        ///
        /// **Particularidad en el cambio de `IsAvailable`:**
        /// Si el estado de un estacionamiento cambia de **`ocupado (false)` a `disponible (true)`**, el sistema interpreta que un vehículo ha salido. En este caso, se realizan las siguientes acciones automáticas:
        /// 1. Se busca el **último registro de uso (`ParkingSpotUsageLog`)** de este estacionamiento que aún no tenga una visita de salida.
        /// 2. Se crea una nueva **entidad `Visit` de tipo "Salida"**, registrando la hora actual de Santiago de Chile, la persona, vehículo y otros datos de la visita de entrada asociada al último uso.
        /// 3. El `Id` de esta nueva `Visit` de salida se asocia al `IdExitVisit` en el `ParkingSpotUsageLog` correspondiente, completando así el ciclo de uso.
        ///
        /// **Validaciones adicionales:**
        /// - El nuevo nombre, si se proporciona, no debe estar repetido dentro del mismo establecimiento (excluyendo el propio estacionamiento que se está actualizando).
        /// - Un usuario `ADMIN` no puede mover un estacionamiento a un `IdEstablishment` diferente al suyo; solo un `SUPERADMIN` puede realizar esta acción.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para UpdateParkingSpotDto:**
        /// ```json
        /// {
        ///   "name": "E-105-Renovado",
        ///   "isAvailable": true, // Ejemplo de cambio de estado a disponible
        ///   "idEstablishment": 1 // Mantener o cambiar el establecimiento (requiere SUPERADMIN si es diferente)
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del espacio de estacionamiento a actualizar.</param>
        /// <param name="dto">
        /// **Objeto DTO con los datos actualizados del espacio de estacionamiento.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`name` (string, opcional):** El nuevo nombre o número identificador del estacionamiento. Si es nulo o vacío, el nombre actual se mantiene.
        /// - **`isAvailable` (bool?, opcional):** El nuevo estado de disponibilidad del estacionamiento (`true` para disponible, `false` para ocupado). Si es nulo, el estado actual se mantiene.
        /// - **`idEstablishment` (int, requerido):** El ID del establecimiento al que pertenece el estacionamiento. **Este campo es obligatorio incluso si no se cambia**, ya que se utiliza para validaciones de permisos.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="ParkingSpot"/> actualizado y 'Message' es "Estacionamiento actualizado correctamente.". Un mensaje de auditoría detallará los cambios.</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos." si el DTO no es válido.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "No tienes permiso para editar este estacionamiento." si el usuario no tiene los permisos para el establecimiento del estacionamiento.
        /// - "No puedes mover este estacionamiento a otro establecimiento." si un usuario que no es SUPERADMIN intenta cambiar el `IdEstablishment`.
        /// </response>
        /// <response code="404">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "Estacionamiento no encontrado." si el ID proporcionado no corresponde a un estacionamiento existente.
        /// - "Establecimiento no encontrado." si el `IdEstablishment` proporcionado en el DTO no existe.
        /// </response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "Ya existe un estacionamiento con ese nombre en este establecimiento." si el nuevo nombre ya está en uso en el mismo establecimiento.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al actualizar el estacionamiento." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] UpdateParkingSpotDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, "Datos inválidos."));

            var response = await _service.UpdateParkingSpotAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Elimina un espacio de estacionamiento de forma física por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Solo los usuarios con permisos para administrar el establecimiento al que pertenece el estacionamiento pueden eliminarlo.
        ///
        /// La eliminación es **física**, lo que significa que el registro del estacionamiento será removido permanentemente de la base de datos.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del espacio de estacionamiento a eliminar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Estacionamiento eliminado correctamente." si la operación fue exitosa.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para eliminar este estacionamiento." si el usuario no tiene los permisos para el establecimiento del estacionamiento.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Estacionamiento no encontrado." si el ID proporcionado no corresponde a un estacionamiento existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al eliminar el estacionamiento." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _service.DeleteParkingSpotAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}