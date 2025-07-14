using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs.Persons;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Services;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// Crea un nuevo registro de persona en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// Se utiliza para registrar la información básica de una persona que interactuará con el sistema,
        /// como visitantes, residentes, o cualquier otra persona relevante.
        /// El número de identificación (`IdentificationNumber`) debe ser único en el sistema.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para PersonDto:**
        /// ```json
        /// {
        ///   "names": "Ana",
        ///   "lastNames": "Gómez",
        ///   "identificationNumber": "12345678-9"
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para la creación de la persona.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`Names` (string, requerido):** Los nombres de la persona.
        /// - **`LastNames` (string, requerido):** Los apellidos de la persona.
        /// - **`IdentificationNumber` (string, requerido):** El número de identificación único de la persona (ej., RUT, pasaporte).
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="Person"/> creado (incluyendo su Id generado automáticamente, Nombres, Apellidos, Número de Identificación) y 'Message' es "Persona creada correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos." si el DTO no es válido. 'Data' puede contener una lista de errores de validación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "Ya existe una persona con ese número de identificación." si ya existe una persona con el mismo número de identificación.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al crear la persona." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePerson([FromBody] PersonDto dto)
        {
            var response = await _personService.CreatePersonAsync(dto);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>
        /// Obtiene una lista de todas las personas registradas en el sistema, filtradas según los permisos del usuario.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Si el usuario es **SUPERADMIN**, la respuesta incluirá todas las personas registradas en el sistema.
        /// - Si el usuario es **ADMIN**, solo se devolverán las personas a las que tiene acceso según la lógica de `_userContext.CanAccessPerson(p)`.
        ///   Esto asegura que un administrador solo vea personas relevantes para su contexto (por ejemplo, personas asociadas a su establecimiento).
        /// </remarks>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos <see cref="Person"/> y 'Message' es "Personas obtenidas correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener las personas." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPersons()
        {
            var response = await _personService.GetAllPersonsAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene los detalles de una persona específica por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Los usuarios con rol **SUPERADMIN** pueden acceder a cualquier persona por su ID.
        /// - Los usuarios con rol **ADMIN** solo pueden ver a las personas a las que tienen acceso, validado por la lógica `_userContext.CanAccessPerson(person)`.
        ///
        /// La respuesta incluirá los detalles completos de la persona, y se intentarán cargar las reservas de áreas comunes a las que ha sido invitada (`InvitedCommonAreaReservations`).
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) de la persona a recuperar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="Person"/> completo con sus relaciones cargadas y 'Message' es "Persona obtenida correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para acceder a esta persona." si el usuario no tiene los permisos para ver los datos de la persona.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Persona no encontrada." si el ID proporcionado no corresponde a una persona existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener la persona." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonById(int id)
        {
            var response = await _personService.GetPersonByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene los detalles de una persona específica buscando por su número de identificación.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Los usuarios con rol **SUPERADMIN** pueden acceder a cualquier persona por su número de identificación.
        /// - Los usuarios con rol **ADMIN** solo pueden ver a las personas a las que tienen acceso, validado por la lógica interna de permisos (`_userContext.CanAccessPerson`).
        ///
        /// La respuesta incluirá los detalles completos de la persona, y se intentarán cargar las reservas de áreas comunes a las que ha sido invitada (`InvitedCommonAreaReservations`).
        /// </remarks>
        /// <param name="identificationNumber">El número de identificación (ej., RUT, pasaporte) de la persona a recuperar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="Person"/> completo con sus relaciones cargadas y 'Message' es "Persona obtenida correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para acceder a esta persona." si el usuario no tiene los permisos para ver los datos de la persona.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Persona no encontrada." si el número de identificación proporcionado no corresponde a ninguna persona existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener la persona." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("idnumber/{identificationNumber}")]
        public async Task<IActionResult> GetPersonByIdentificationNumber(string identificationNumber)
        {
            var response = await _personService.GetPersonByIdentificationNumberAsync(identificationNumber);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Actualiza la información de una persona existente en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// Solo los usuarios con permisos adecuados (`_userContext.CanAccessPerson`) pueden modificar los datos de una persona.
        /// Se validará que el número de identificación (`IdentificationNumber`) proporcionado no esté ya en uso por otra persona en el sistema.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para PersonDto:**
        /// ```json
        /// {
        ///   "names": "Ana María",
        ///   "lastNames": "Gómez Pérez",
        ///   "identificationNumber": "12.345.678-K"
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">El **ID** de la persona que se desea actualizar.</param>
        /// <param name="dto">
        /// **Objeto DTO con los datos actualizados de la persona.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`Names` (string, requerido):** Los nombres actualizados de la persona.
        /// - **`LastNames` (string, requerido):** Los apellidos actualizados de la persona.
        /// - **`IdentificationNumber` (string, requerido):** El número de identificación actualizado de la persona. Debe ser único.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="Person"/> actualizado y 'Message' es "Persona actualizada correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Ya existe otra persona con ese número de identificación." si el número de identificación ya está en uso.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para modificar esta persona." si el usuario no tiene los permisos para acceder o modificar a la persona.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Persona no encontrada." si el `id` proporcionado no corresponde a ninguna persona existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al actualizar la persona." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] PersonDto dto)
        {
            var response = await _personService.UpdatePersonAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Elimina una persona del sistema de forma física por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// **Reglas de eliminación:**
        /// - Solo los usuarios con permisos adecuados (`_userContext.CanAccessPerson`) pueden eliminar una persona.
        /// - Una persona **no puede ser eliminada** si tiene reservas de áreas comunes asociadas como invitado (`InvitedCommonAreaReservations`).
        ///   Esto es para preservar la integridad referencial y el historial de eventos.
        /// - La eliminación es **física**, lo que significa que el registro de la persona será removido permanentemente de la base de datos.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) de la persona a eliminar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Persona eliminada correctamente." si la operación fue exitosa.</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "No se puede eliminar la persona porque tiene reservas asociadas como invitado." si existen dependencias que impiden la eliminación.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para eliminar esta persona." si el usuario no tiene los permisos para acceder o eliminar a la persona.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Persona no encontrada." si el ID proporcionado no corresponde a ninguna persona existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al eliminar la persona." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var response = await _personService.DeletePersonAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}

