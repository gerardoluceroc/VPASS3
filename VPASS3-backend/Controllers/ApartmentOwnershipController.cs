using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.ApartmentOwnerships;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApartmentOwnershipController : ControllerBase
    {
        private readonly IApartmentOwnershipService _apartmentOwnershipService;
        private readonly IUserContextService _userContext;

        public ApartmentOwnershipController(IApartmentOwnershipService apartmentOwnershipService, IUserContextService userContext)
        {
            _apartmentOwnershipService = apartmentOwnershipService;
            _userContext = userContext;
        }

        /// <summary>
        /// Asigna una persona como propietario o inquilino actual de un departamento específico.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// Este proceso crea un nuevo registro de `ApartmentOwnership` marcando el inicio de la relación. La `StartDate` se establece
        /// automáticamente con la hora actual de Santiago de Chile al momento de la creación. La `EndDate` se dejará nula,
        /// indicando que esta es la propiedad/inquilinato activo.
        ///
        /// **Validaciones clave:**
        /// - El **departamento** y la **persona** especificados deben existir.
        /// - El usuario debe tener permisos para asignar un propietario en el establecimiento al que pertenece el departamento.
        ///   - **ADMIN**: Solo puede asignar propietarios en su propio establecimiento.
        ///   - **SUPERADMIN**: Puede asignar propietarios en cualquier establecimiento.
        /// - Se verificará que el departamento **no tenga ya un propietario/inquilino activo** (es decir, un `ApartmentOwnership` sin `EndDate`).
        ///   Si ya existe uno, la operación fallará para evitar múltiples propietarios activos concurrentes.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para CreateApartmentOwnershipDto:**
        /// ```json
        /// {
        ///   "idApartment": 101, // ID del departamento al que se asignará el propietario
        ///   "idPerson": 50     // ID de la persona que será el propietario/inquilino
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los IDs del departamento y la persona a asignar.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`IdApartment` (int, requerido):** El ID del departamento al que se va a asignar el propietario.
        /// - **`IdPerson` (int, requerido):** El ID de la persona que se va a asignar como propietario/inquilino.
        /// </param>
        /// <returns>Un <see cref="ActionResult"/> de tipo <see cref="ResponseDto"/> que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="ApartmentOwnership"/> creado y 'Message' es "Propietario asignado correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con un mensaje de error si los datos son inválidos.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para asignar un propietario en este establecimiento." si el rol del usuario no lo permite.</response>
        /// <response code="404">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "Departamento no encontrado."
        /// - "Zona no encontrada para el departamento." (indicando un problema de integridad en la base de datos).
        /// - "Persona no encontrada."
        /// </response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando que el departamento ya tiene un propietario activo, evitando duplicidades.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando un error en el servidor al asignar el propietario.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateApartmentOwnershipDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _apartmentOwnershipService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene una lista de todos los registros de inquilinos (ApartmentOwnerships) en el sistema,
        /// filtrados según los permisos del usuario autenticado.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// **Comportamiento según el rol del usuario:**
        /// - Si el usuario es **SUPERADMIN**: La respuesta incluirá todos los registros de inquilinos de todos los establecimientos.
        /// - Si el usuario es **ADMIN**: Solo se devolverán los registros de inquilinos de los departamentos que pertenecen a su establecimiento asociado.
        ///
        /// La respuesta de cada registro de inquilino incluirá datos detallados de sus relaciones:
        /// - Información del departamento (`Apartment`) al que pertenece.
        /// - Información de la zona (`Zone`) a la que pertenece el departamento.
        /// - Información de la persona (`Person`) que es o fue el inquilino.
        ///
        /// Los registros incluirán tanto los inquilinatos actuales (aquellos sin `EndDate`) como los históricos (con `EndDate` definida).
        /// </remarks>
        /// <returns>Un <see cref="ActionResult"/> de tipo <see cref="ResponseDto"/> que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos <see cref="ApartmentOwnership"/> y 'Message' es "Propiedades obtenidas correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes un establecimiento asociado." si el usuario no es SUPERADMIN y no tiene un ID de establecimiento asignado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener las propiedades." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _apartmentOwnershipService.GetAllAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene los detalles de un registro específico de inquilino (ApartmentOwnership) buscando por su ID único.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// **Validaciones clave:**
        /// - El registro `ApartmentOwnership` con el ID proporcionado debe existir.
        /// - El usuario autenticado debe tener permisos para acceder a este registro:
        ///   - **ADMIN**: Solo puede ver registros de inquilinos que pertenezcan a su propio establecimiento.
        ///   - **SUPERADMIN**: Puede ver cualquier registro de inquilino en el sistema.
        ///
        /// La respuesta incluirá los detalles completos del registro de inquilino, cargando automáticamente las relaciones con:
        /// - El departamento (`Apartment`) al que se asocia.
        /// - La zona (`Zone`) a la que pertenece el departamento.
        /// - La persona (`Person`) asociada a este registro de inquilinato.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del registro de inquilino a recuperar.</param>
        /// <returns>Un <see cref="ActionResult"/> de tipo <see cref="ResponseDto"/> que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="ApartmentOwnership"/> completo con sus relaciones cargadas.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para ver este registro." si el usuario no tiene los permisos necesarios o no pertenece al establecimiento.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Registro no encontrado." si el ID proporcionado no corresponde a un `ApartmentOwnership` existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando un error en el servidor al obtener el registro.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _apartmentOwnershipService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Finaliza la relación de inquilinato (propiedad) actual para un departamento específico,
        /// registrando la fecha de término en el historial de `ApartmentOwnership`.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// La operación busca el registro de `ApartmentOwnership` activo (aquel sin `EndDate`) para el `IdApartment` proporcionado.
        /// Si se encuentra, se establece su `EndDate` a la fecha y hora actual de Santiago de Chile, marcando así el fin de esa relación.
        ///
        /// **Validaciones clave:**
        /// - El **departamento** especificado debe existir.
        /// - El usuario debe tener permisos para modificar propiedades en el establecimiento al que pertenece el departamento.
        ///   - **ADMIN**: Solo puede finalizar propiedades en su propio establecimiento.
        ///   - **SUPERADMIN**: Puede finalizar propiedades en cualquier establecimiento.
        /// - Se verificará que el departamento **tenga un propietario/inquilino activo** al que finalizar. Si no hay ninguno,
        ///   la operación indica que el departamento ya está "desocupado" o no tiene un propietario actual.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para EndApartmentOwnershipDto:**
        /// ```json
        /// {
        ///   "idApartment": 101 // ID del departamento cuyo inquilino actual se va a finalizar
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con el ID del departamento.**
        /// Se espera un objeto JSON con la siguiente propiedad:
        /// - **`IdApartment` (int, requerido):** El ID del departamento cuya relación de propiedad/inquilinato actual se desea finalizar.
        /// </param>
        /// <returns>Un <see cref="ActionResult"/> de tipo <see cref="ResponseDto"/> que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="ApartmentOwnership"/> actualizado (con `EndDate`) y 'Message' es "Propiedad finalizada correctamente.".</response>
        /// <response code="400">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - Los datos son inválidos.
        /// - "Zona no encontrada para el departamento." (indicando un problema de integridad en la base de datos).
        /// </response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para editar propiedad en este establecimiento." si el rol del usuario no lo permite.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Departamento no encontrado.".</response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "Este departamento no tiene un propietario activo." si no hay un `ApartmentOwnership` sin `EndDate` para el departamento.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando un error en el servidor al finalizar la propiedad.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("end")]
        public async Task<ActionResult<ResponseDto>> EndOwnership([FromBody] EndApartmentOwnershipDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _apartmentOwnershipService.EndCurrentOwnershipAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Elimina un registro de inquilino (ApartmentOwnership) del sistema de forma permanente por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// **Reglas de eliminación:**
        /// - El registro `ApartmentOwnership` con el ID proporcionado debe existir.
        /// - El usuario autenticado debe tener permisos para eliminar este registro:
        ///   - **ADMIN**: Solo puede eliminar registros de inquilinos que pertenezcan a su propio establecimiento.
        ///   - **SUPERADMIN**: Puede eliminar cualquier registro de inquilino en el sistema.
        /// - La eliminación es **física**, lo que significa que el registro será removido permanentemente de la base de datos.
        /// - La acción de eliminación se registra en el log de auditoría.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del registro de inquilino a eliminar.</param>
        /// <returns>Un <see cref="ActionResult"/> de tipo <see cref="ResponseDto"/> que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Registro eliminado correctamente." si la operación fue exitosa.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para eliminar este registro." si el usuario no tiene los permisos necesarios o no pertenece al establecimiento.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Registro no encontrado." si el ID proporcionado no corresponde a un `ApartmentOwnership` existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando un error en el servidor al eliminar el registro.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _apartmentOwnershipService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
