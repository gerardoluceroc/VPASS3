using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using VPASS3_backend.Filters;
using VPASS3_backend.DTOs.Apartments;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApartmentController : ControllerBase
    {
        private readonly IApartmentService _apartmentService;

        public ApartmentController(IApartmentService apartmentService)
        {
            _apartmentService = apartmentService;
        }

        /// <summary>
        /// Crea un nuevo departamento dentro de una zona específica.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// Los usuarios solo pueden crear departamentos en zonas asociadas a su propio establecimiento, a menos que tengan el rol SUPERADMIN.
        /// El nombre del departamento debe ser único dentro de la zona a la que pertenece y no debe haber sido marcado como eliminado previamente.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para ApartmentDto:**
        /// ```json
        /// {
        ///   "name": "101",
        ///   "idZone": 5
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para la creación del departamento.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`name` (string, requerido):** El nombre del nuevo departamento (ej., "101").
        /// - **`idZone` (int, requerido):** El ID de la zona a la que pertenecerá este departamento.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto Apartment creado (incluyendo su Id generado automáticamente, Name, IdZone, IsDeleted), y 'Message' es "Departamento creado correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos. Verifica los campos ingresados." si el DTO no es válido. 'Data' puede contener una lista de errores de validación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos para crear departamentos en la zona especificada.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Zona asociada no encontrada." si el IdZone proporcionado no existe.</response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "Ya existe un departamento con ese nombre en esta zona." si ya hay un departamento activo con el mismo nombre en la misma zona.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al crear el departamento." si ocurre un error interno.</response>
        [HttpPost("create")]
        [Authorize(Policy = "ManageOwnProfile")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] ApartmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _apartmentService.CreateApartmentAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene una lista de todos los departamentos activos, incluyendo el inquilino actual si existe.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// Si el usuario es **SUPERADMIN**, se retornarán todos los departamentos activos de todos los establecimientos.
        /// Si el usuario es **ADMIN**, solo se retornarán los departamentos activos de las zonas que pertenecen a su establecimiento asociado.
        ///
        /// La respuesta para cada departamento incluirá su información básica (ID, nombre, ID de zona, estado de eliminación),
        /// el nombre de la zona a la que pertenece, y los datos del **inquilino actualmente activo** (`ActiveOwnership`).
        /// Si un departamento no tiene un inquilino activo en este momento, la propiedad `ActiveOwnership` será `null`.
        /// El objeto `ApartmentOwnership` contendrá información de la persona asociada.
        /// </remarks>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos <see cref="ApartmentWithCurrentOwnerDto"/>. Cada objeto incluye la información del departamento y, opcionalmente, los detalles del inquilino activo.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los roles ADMIN o SUPERADMIN, o si un ADMIN no tiene un establecimiento asociado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener los departamentos." si ocurre un error interno.</response>
        [HttpGet("all")]
        [Authorize(Policy = "ManageOwnProfile")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _apartmentService.GetAllApartmentsAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene los detalles de un departamento específico por su ID, incluyendo el inquilino actual si existe.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// Los usuarios solo pueden acceder a departamentos que estén asociados a su propio establecimiento, a menos que tengan el rol SUPERADMIN.
        /// Solo se recuperarán los departamentos **activos** (aquellos que no estén marcados como eliminados). La respuesta incluirá
        /// la información básica del departamento (ID, nombre, ID de zona, estado de eliminación), el nombre de la zona a la que pertenece,
        /// y los datos del **inquilino actualmente activo** (`ActiveOwnership`). Si el departamento no tiene un inquilino activo,
        /// la propiedad `ActiveOwnership` será `null`. El objeto `ApartmentOwnership` contendrá información de la persona asociada.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del departamento a recuperar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene un objeto <see cref="ApartmentWithCurrentOwnerDto"/> con los detalles del departamento y, opcionalmente, los detalles del inquilino activo.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos para acceder a este departamento.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Departamento no encontrado." si el departamento con el ID proporcionado no existe o está marcado como eliminado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener el departamento." si ocurre un error interno del servidor.</response>
        [HttpGet("{id}")]
        [Authorize(Policy = "ManageOwnProfile")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _apartmentService.GetApartmentByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Actualiza la información de un departamento existente por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// El usuario debe tener permisos para administrar el departamento que intenta actualizar.
        /// Permite modificar el nombre del departamento y reasignarlo a una zona diferente dentro del mismo establecimiento (o a un establecimiento distinto si es SUPERADMIN).
        /// El nombre del departamento debe ser único dentro de su nueva zona para departamentos activos.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para ApartmentDto:**
        /// ```json
        /// {
        ///   "name": "202",
        ///   "idZone": 6 // ID de la nueva zona (ej. "Piso 2")
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del departamento a actualizar.</param>
        /// <param name="dto">
        /// **Objeto DTO con los datos actualizados del departamento.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`name` (string, requerido):** El nuevo nombre para el departamento.
        /// - **`idZone` (int, requerido):** El ID de la zona a la que pertenecerá el departamento.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto Apartment actualizado (Id, Name, IdZone, IsDeleted), y 'Message' es "Departamento actualizado correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos. Verifica los campos ingresados." si el DTO no es válido. 'Data' puede contener una lista de errores de validación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos para editar este departamento.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Departamento no encontrado." si el ID no corresponde a un departamento existente o está marcado como eliminado, o "Zona asociada no encontrada." si el IdZone proporcionado no existe.</response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "Ya existe un departamento con ese nombre en esta zona." si ya existe un departamento activo con el mismo nombre en la zona especificada (excluyendo el departamento que se está actualizando).</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al actualizar el departamento." si ocurre un error interno.</response>
        [HttpPut("update/{id}")]
        [Audit("Actualización de información de departamento")]
        [Authorize(Policy = "ManageOwnProfile")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] ApartmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _apartmentService.UpdateApartmentAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Elimina lógicamente un departamento por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// El usuario debe tener permisos para eliminar el departamento.
        ///
        /// La eliminación es **lógica**, lo que significa que el departamento se marcará como `IsDeleted = true` en la base de datos,
        /// pero no se eliminará físicamente. Esto mantiene un registro histórico del departamento.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del departamento a eliminar lógicamente.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Departamento eliminado correctamente." si la operación fue exitosa.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con un mensaje de error si el usuario no tiene los permisos para eliminar este departamento.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Departamento no encontrado." si el departamento con el ID proporcionado no existe o ya estaba marcado como eliminado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al eliminar el departamento." si ocurre un error interno.</response>
        [HttpDelete("delete/{id}")]
        [Audit("Eliminación de departamento")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _apartmentService.DeleteApartmentAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}