using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.Zones;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using VPASS3_backend.Filters;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZoneController : ControllerBase
    {
        private readonly IZoneService _zoneService;
        private readonly IUserContextService _userContext;

        public ZoneController(IZoneService zoneService, IUserContextService userContext)
        {
            _zoneService = zoneService;
            _userContext = userContext;
        }

        /// <summary>
        /// Crea una nueva zona (ej. piso) dentro de un establecimiento específico.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// El usuario debe tener permisos para administrar el establecimiento al que se intenta asociar la zona.
        /// El nombre de la zona debe ser único dentro del mismo establecimiento y la zona no debe haber sido marcada como eliminada previamente.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para CreateZoneDto:**
        /// ```json
        /// {
        ///   "name": "Piso 5",
        ///   "establishmentId": 1
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para la creación de la zona.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`name` (string, requerido):** El nombre de la nueva zona (ej., "Piso 1", "Sótano").
        /// - **`establishmentId` (int, requerido):** El ID del establecimiento al que pertenecerá esta zona.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto Zone creado (incluyendo su Id generado automáticamente, Name, EstablishmentId, IsDeleted), y 'Message' es "Zona creada correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos. Verifica los campos ingresados." si el DTO no es válido. 'Data' puede contener una lista de errores de validación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos para administrar el establecimiento o la zona.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Establecimiento no encontrado." si el EstablishmentId proporcionado no existe.</response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "Ya existe una zona con ese nombre en este establecimiento." si ya hay una zona activa con el mismo nombre en el mismo establecimiento.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al crear la zona." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Creación de zona")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateZoneDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            // Se verifica que el usuario sea Super Admin o que esté consultando a recursos relacionados con su usuario
            if (!_userContext.CanAccessOwnEstablishment(dto.EstablishmentId))
            {
                return StatusCode(403, new ResponseDto(403, message: "No cuenta con los permisos para administrar la información de otros usuarios"));
            }

            var response = await _zoneService.CreateZoneAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene una lista de todas las zonas activas en el sistema, filtradas por los permisos del usuario.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// Si el usuario es **SUPERADMIN**, se retornarán todas las zonas activas de todos los establecimientos.
        /// Si el usuario es **ADMIN**, solo se retornarán las zonas activas de su establecimiento asociado.
        ///
        /// Las zonas retornadas incluirán sus relaciones con el establecimiento al que pertenecen y sus departamentos asociados (solo los que no estén marcados como eliminados).
        /// </remarks>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos Zone (solo los no eliminados). Cada Zone incluye su Id, Name, EstablishmentId, IsDeleted, y las colecciones de Establishment y Apartments (solo los no eliminados).</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los roles ADMIN o SUPERADMIN, o si un ADMIN no tiene un establecimiento asociado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener las zonas." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _zoneService.GetAllZonesAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene los detalles de una zona específica por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (siguiendo la política "ManageOwnProfile").
        ///
        /// Los usuarios solo pueden acceder a las zonas asociadas con su propio establecimiento, a menos que tengan el rol **SUPERADMIN**.
        /// Solo se recuperarán las zonas **activas** (aquellas que no estén marcadas como eliminadas). La respuesta incluirá el establecimiento asociado a la zona y sus departamentos activos.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) de la zona a recuperar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto Zone (Id, Name, EstablishmentId, IsDeleted, además de sus colecciones de Establishment y Apartments activos), y 'Message' es "Zona obtenida correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos para acceder a la zona solicitada.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Zona no encontrada." si la zona con el ID proporcionado no existe o está marcada como eliminada.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener la zona." si ocurre un error interno del servidor.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _zoneService.GetZoneByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Actualiza la información de una zona (piso) existente por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// El usuario debe tener permisos para administrar la zona que intenta actualizar.
        /// Permite modificar el nombre de la zona y, opcionalmente, reasignarla a un establecimiento diferente.
        /// El nombre de la zona debe ser único dentro del mismo establecimiento para zonas activas.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para CreateZoneDto (usado para actualización):**
        /// ```json
        /// {
        ///   "name": "Piso 10",
        ///   "establishmentId": 2 // Opcional: para cambiar el establecimiento asociado
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) de la zona a actualizar.</param>
        /// <param name="dto">
        /// **Objeto DTO con los datos actualizados de la zona.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`name` (string, requerido):** El nuevo nombre para la zona.
        /// - **`establishmentId` (int, requerido):** El ID del establecimiento al que pertenecerá la zona.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto Zone actualizado (Id, Name, EstablishmentId, IsDeleted), y 'Message' es "Zona actualizada correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos. Verifica los campos ingresados." si el DTO no es válido. 'Data' puede contener una lista de errores de validación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos para acceder a la zona o al establecimiento.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Zona no encontrada." si la zona con el ID proporcionado no existe o está marcada como eliminada, o "Establecimiento no encontrado." si el EstablishmentId proporcionado no existe.</response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "Ya existe una zona con ese nombre en este establecimiento." si ya existe una zona activa con el mismo nombre en el mismo establecimiento (excluyendo la zona que se está actualizando).</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al actualizar la zona." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Actualización de información de zona")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] CreateZoneDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _zoneService.UpdateZoneAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Elimina lógicamente una zona y todos sus departamentos (Apartments) asociadas por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// El usuario debe tener permisos para administrar la zona que intenta eliminar.
        /// La eliminación es **lógica**, lo que significa que la zona y sus departamentos se marcarán como `IsDeleted = true` en la base de datos,
        /// pero no se eliminarán físicamente.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) de la zona a eliminar lógicamente.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Zona y departamentos eliminados lógicamente." si la operación fue exitosa.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos para eliminar la zona.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Zona no encontrada o ya eliminada." si la zona con el ID proporcionado no existe o ya estaba marcada como eliminada.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al eliminar la zona." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Eliminación de zona")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _zoneService.DeleteZoneAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}