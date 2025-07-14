using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.Visits;
using VPASS3_backend.Filters;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VisitController : ControllerBase
    {
        private readonly IVisitService _visitService;

        public VisitController(IVisitService visitService)
        {
            _visitService = visitService;
        }

        /// <summary>
        /// Registra una nueva visita en el sistema, que puede ser de entrada o de salida.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// La dirección de la visita (entrada o salida) se determina por el `IdDirection` proporcionado.
        /// Se realizan múltiples validaciones para asegurar la integridad de los datos y los permisos del usuario.
        ///
        /// **Comportamiento según el rol del usuario:**
        /// - Si el usuario es **ADMIN**: El `EstablishmentId` se toma automáticamente del contexto del usuario.
        /// - Si el usuario es **SUPERADMIN**: Debe proporcionar el `EstablishmentId` en el cuerpo de la petición (`dto`).
        ///
        /// **Manejo de Vehículos y Estacionamientos:**
        /// - Si `VehicleIncluded` es `true`, se requiere `IdParkingSpot` y se actualiza la disponibilidad del estacionamiento:
        ///   - Si es una **entrada**: El estacionamiento se marca como **no disponible**.
        ///   - Si es una **salida**: El estacionamiento se marca como **disponible**.
        ///
        /// **Validaciones clave:**
        /// - El establecimiento, persona, tipo de visita, zona y (si aplica) departamento y estacionamiento deben existir.
        /// - La persona no debe estar en la lista negra del establecimiento.
        /// - El usuario debe tener permisos para acceder a las entidades relacionadas (establecimiento, tipo de visita, zona, departamento, estacionamiento).
        /// - Si se selecciona un estacionamiento para una entrada, este debe estar disponible.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para VisitDto (Entrada con vehículo):**
        /// ```json
        /// {
        ///   "establishmentId": 1,        // Solo necesario si el usuario es SUPERADMIN
        ///   "idPerson": 101,
        ///   "zoneId": 20,
        ///   "idDirection": 1,            // Suponiendo 1 = Entrada
        ///   "idApartment": 202,          // Opcional, si la visita es a un apartamento específico
        ///   "vehicleIncluded": true,
        ///   "authorizedTime": "01:30:00", // 1 hora y 30 minutos
        ///   "licensePlate": "ABCD-12",
        ///   "idParkingSpot": 5,
        ///   "idVisitType": 3             // Por ejemplo, "Visita Normal"
        /// }
        /// ```
        /// **Ejemplo de cuerpo de petición (Request Body) para VisitDto (Salida sin vehículo):**
        /// ```json
        /// {
        ///   "establishmentId": 1,
        ///   "idPerson": 101,
        ///   "zoneId": 20,
        ///   "idDirection": 2,            // Suponiendo 2 = Salida
        ///   "idApartment": 202,
        ///   "vehicleIncluded": false,
        ///   "authorizedTime": null,
        ///   "licensePlate": null,
        ///   "idParkingSpot": null,
        ///   "idVisitType": 3
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para el registro de la visita.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`EstablishmentId` (int?, opcional):** ID del establecimiento. **Obligatorio para SUPERADMIN, ignorado para ADMIN.**
        /// - **`IdPerson` (int, requerido):** ID de la persona que realiza la visita.
        /// - **`ZoneId` (int, requerido):** ID de la zona de destino de la visita (ej., piso, torre).
        /// - **`IdDirection` (int, requerido):** ID de la dirección de la visita (ej., 1 para Entrada, 2 para Salida).
        /// - **`IdApartment` (int?, opcional):** ID del apartamento de destino, si aplica.
        /// - **`VehicleIncluded` (bool, requerido):** Indica si la visita incluye un vehículo.
        /// - **`AuthorizedTime` (TimeSpan?, opcional):** Tiempo autorizado de uso del estacionamiento, si `VehicleIncluded` es `true`.
        /// - **`LicensePlate` (string?, opcional):** Patente del vehículo, si `VehicleIncluded` es `true`.
        /// - **`IdParkingSpot` (int?, opcional):** ID del estacionamiento asignado, si `VehicleIncluded` es `true`.
        /// - **`IdVisitType` (int, requerido):** ID del tipo de visita (ej., "Visita normal", "Técnico").
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="Visit"/> creado y 'Message' es "Visita creada correctamente.".</response>
        /// <response code="400">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "Debes especificar el ID del establecimiento." (para SUPERADMIN si no se proporciona).
        /// - "Debes especificar un estacionamiento si el vehículo está incluido."
        /// - "El estacionamiento seleccionado se encuentra ocupado." (si es una entrada y el estacionamiento no está disponible).
        /// </response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "Rol no autorizado para crear visitas."
        /// - "No se encontró el establecimiento asociado al usuario." (para ADMIN si no tiene).
        /// - "La persona se encuentra en la lista negra del establecimiento y no puede ingresar."
        /// - "No tienes acceso al tipo de visita o no corresponde al establecimiento."
        /// - "No tienes acceso a la zona o no pertenece al establecimiento."
        /// - "No tienes acceso al departamento especificado."
        /// - "No tienes acceso al estacionamiento especificado."
        /// </response>
        /// <response code="404">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "El establecimiento especificado no existe."
        /// - "El tipo de visita especificado no existe."
        /// - "La zona especificada no existe."
        /// - "El departamento especificado no existe o no pertenece a la zona."
        /// - "El estacionamiento especificado no existe."
        /// </response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al crear la visita." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Registro de visita")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateVisit([FromBody] VisitDto dto)
        {
            var response = await _visitService.CreateVisitAsync(dto);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>
        /// Exporta los registros de visitas a un archivo Excel (.xlsx) para un rango de fechas determinado.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// **Comportamiento según el rol del usuario:**
        /// - Si el usuario es **ADMIN**: El reporte incluirá solo las visitas de su establecimiento asociado.
        /// - Si el usuario es **SUPERADMIN**: El reporte incluirá las visitas de todos los establecimientos.
        ///
        /// **Contenido del reporte:**
        /// El archivo Excel contendrá columnas con la siguiente información para cada visita:
        /// - ID Visita
        /// - Fecha y Hora (de entrada)
        /// - Visitante (Nombre y Apellidos)
        /// - Tipo de Visita
        /// - Establecimiento
        /// - Destino (Zona - Departamento)
        /// - Dirección (Entrada/Salida)
        /// - Vehículo (Sí/No)
        /// - Patente
        /// - Estacionamiento
        ///
        /// **Validaciones:**
        /// - La `StartDate` no puede ser posterior a la `EndDate`.
        /// - Si el usuario es ADMIN, debe tener un establecimiento asociado.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para GetVisitByDatesDto:**
        /// ```json
        /// {
        ///   "startDate": "2024-01-01",
        ///   "endDate": "2024-01-31"
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con el rango de fechas para la exportación.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`StartDate` (DateOnly, requerido):** La fecha de inicio del rango (formato 'YYYY-MM-DD').
        /// - **`EndDate` (DateOnly, requerido):** La fecha de fin del rango (formato 'YYYY-MM-DD').
        /// </param>
        /// <returns>Un <see cref="ActionResult"/> que contiene el archivo Excel si la operación fue exitosa, o un <see cref="ResponseDto"/> en caso de error.</returns>
        /// <response code="200">Retorna un archivo Excel (`.xlsx`) con el reporte de visitas. El 'Message' del `ResponseDto` indicará si se encontraron datos o no.</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos. Verifica las fechas ingresadas." o "La fecha de inicio no puede ser posterior a la fecha final.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes un establecimiento asociado." si el usuario ADMIN no tiene un establecimiento asignado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al generar el reporte de visitas." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("export/excel/byDates")]
        public async Task<IActionResult> ExportVisitsToExcel([FromBody] GetVisitByDatesDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica las fechas ingresadas."));
            }

            var response = await _visitService.ExportVisitsToExcelAsync(dto);

            if (response.StatusCode != 200 || response.Data == null)
            {
                return StatusCode(response.StatusCode, response);
            }

            // Extraer los datos del archivo del response
            var fileData = (dynamic)response.Data;
            byte[] fileContents = fileData.FileContent;
            string contentType = fileData.ContentType;
            string fileName = fileData.FileName;

            return File(fileContents, contentType, fileName);
        }

        /// <summary>
        /// Exporta los registros de visitas a un archivo Excel (.xlsx) filtrados por el número de identificación (RUT) de una persona.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// **Comportamiento según el rol del usuario:**
        /// - Si el usuario es **ADMIN**: El reporte incluirá solo las visitas de la persona que se encuentren en su establecimiento asociado.
        /// - Si el usuario es **SUPERADMIN**: El reporte incluirá todas las visitas de la persona, de todos los establecimientos.
        ///
        /// **Contenido del reporte:**
        /// El archivo Excel contendrá las mismas columnas detalladas en la exportación por rango de fechas:
        /// - ID Visita, Fecha y Hora, Visitante, Tipo de Visita, Establecimiento, Destino, Dirección, Vehículo, Patente, Estacionamiento.
        ///
        /// **Validaciones:**
        /// - El número de identificación (`IdentificationNumber` o RUT) proporcionado debe corresponder a una persona existente en el sistema.
        /// - Si el usuario es ADMIN, debe tener un establecimiento asociado.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para ExportVisitsExcelByIdentificationNumberDto:**
        /// ```json
        /// {
        ///   "identificationNumber": "12.345.678-9" // Ejemplo de RUT chileno
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con el número de identificación de la persona.**
        /// Se espera un objeto JSON con la siguiente propiedad:
        /// - **`IdentificationNumber` (string, requerido):** El número de identificación (RUT) de la persona cuyas visitas se desean exportar.
        /// </param>
        /// <returns>Un <see cref="ActionResult"/> que contiene el archivo Excel si la operación fue exitosa, o un <see cref="ResponseDto"/> en caso de error.</returns>
        /// <response code="200">Retorna un archivo Excel (`.xlsx`) con el reporte de visitas. El 'Message' del `ResponseDto` indicará si se encontraron datos o no.</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos. Verifica el RUT ingresado.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes un establecimiento asociado." si el usuario ADMIN no tiene un establecimiento asignado.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Visitante no encontrado." si el RUT proporcionado no corresponde a ninguna persona.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al generar el reporte de visitas." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("export/excel/byRut")]
        public async Task<IActionResult> ExportVisitsToExcelByIdentificationNumber([FromBody] ExportVisitsExcelByIdentificationNumberDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica el RUT ingresado."));
            }

            var response = await _visitService.ExportVisitsToExcelByIdentificationNumberAsync(dto);

            if (response.StatusCode != 200 || response.Data == null)
            {
                return StatusCode(response.StatusCode, response);
            }

            // Extraer los datos del archivo del response
            var fileData = (dynamic)response.Data;
            byte[] fileContents = fileData.FileContent;
            string contentType = fileData.ContentType;
            string fileName = fileData.FileName;

            return File(fileContents, contentType, fileName);
        }

        /// <summary>
        /// Obtiene una lista de todos los registros de visitas en el sistema, filtrados según los permisos del usuario autenticado.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Si el usuario es **SUPERADMIN**, la respuesta incluirá todos los registros de visitas de todos los establecimientos.
        /// - Si el usuario es **ADMIN**, solo se devolverán los registros de visitas que pertenecen a su establecimiento asociado.
        ///   La lógica de `_userContext.CanAccessVisit(v)` asegura que el administrador solo acceda a los registros de su propio contexto.
        ///
        /// La respuesta de cada visita incluirá datos detallados de sus relaciones:
        /// - Información del estacionamiento (`ParkingSpot`).
        /// - Tipo de visita (`VisitType`).
        /// - Dirección de la visita (entrada/salida) (`Direction`).
        /// - Zona de destino (`Zone`).
        /// - Persona que realiza la visita (`Person`).
        /// - Apartamento de destino (`Apartment`), si aplica.
        ///
        /// **Nota importante:** Para evitar ciclos de referencia y mantener la respuesta concisa, los detalles de los apartamentos anidados dentro de la `Zone` de cada visita se limpian (`visit.Zone.Apartments.Clear()`) antes de enviar la respuesta.
        /// </remarks>
        /// <returns>Un <see cref="ActionResult"/> de tipo <see cref="ResponseDto"/> que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos <see cref="Visit"/> y 'Message' es "Visitas obtenidas correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes un establecimiento asociado." si el usuario no es SUPERADMIN y no tiene un ID de establecimiento asignado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener las visitas." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllVisits()
        {
            var response = await _visitService.GetAllVisitsAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obtiene los detalles de un registro de visita específico buscando por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// Se realizarán las siguientes validaciones:
        /// - La visita debe existir en la base de datos.
        /// - El usuario autenticado debe tener permisos para acceder a esta visita, lo cual se determina mediante la lógica `_userContext.CanAccessVisit(visit)`. Esto generalmente significa que un `ADMIN` solo puede ver visitas de su propio establecimiento, mientras que un `SUPERADMIN` puede ver todas.
        ///
        /// La respuesta incluirá los detalles completos de la visita, cargando automáticamente las relaciones con:
        /// - El estacionamiento (`ParkingSpot`).
        /// - El tipo de visita (`VisitType`).
        /// - La dirección (entrada/salida) (`Direction`).
        /// - La zona de destino (`Zone`).
        /// - La persona visitante (`Person`).
        /// - El apartamento de destino (`Apartment`), si aplica.
        ///
        /// **Nota importante:** Para evitar ciclos de referencia y mantener la respuesta concisa, los detalles de los apartamentos anidados dentro de la `Zone` de la visita se limpian (`visit.Zone.Apartments.Clear()`) antes de enviar la respuesta.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del registro de visita a recuperar.</param>
        /// <returns>Un <see cref="ActionResult"/> de tipo <see cref="ResponseDto"/> que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="Visit"/> completo con sus relaciones cargadas y 'Message' es "Visita obtenida correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para acceder a esta visita." si el usuario no tiene los permisos necesarios.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Visita no encontrada." si el ID proporcionado no corresponde a un registro de visita existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener la visita." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVisitById(int id)
        {
            var response = await _visitService.GetVisitByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Actualiza la información de un registro de visita existente en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// **Comportamiento según el rol del usuario:**
        /// - Si el usuario es **ADMIN**: El `EstablishmentId` se toma automáticamente del contexto del usuario.
        /// - Si el usuario es **SUPERADMIN**: Debe proporcionar el `EstablishmentId` en el cuerpo de la petición (`dto`).
        ///
        /// **Validaciones y restricciones:**
        /// - Solo los usuarios con permisos para acceder a la visita (`_userContext.CanAccessVisit`) pueden modificarla.
        /// - Un usuario **ADMIN no puede reasignar** la visita a un establecimiento diferente al suyo; esta operación está restringida para `SUPERADMIN`.
        /// - El establecimiento, persona, tipo de visita, zona y (si aplica) departamento y estacionamiento deben existir y ser accesibles por el usuario.
        /// - Si se incluye un vehículo (`VehicleIncluded`), se requiere un `IdParkingSpot`. No se valida la disponibilidad del estacionamiento en la actualización.
        ///
        /// **Importante:** La `EntryDate` de la visita no se actualiza a través de este endpoint, ya que representa el momento original del registro.
        /// Este endpoint se enfoca en corregir o modificar los detalles de una visita ya registrada.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para VisitDto:**
        /// ```json
        /// {
        ///   "establishmentId": 1,        // Solo necesario si el usuario es SUPERADMIN y/o se cambia de establecimiento.
        ///   "idPerson": 101,
        ///   "zoneId": 20,
        ///   "idDirection": 1,            // Suponiendo 1 = Entrada
        ///   "idApartment": 202,          // Opcional
        ///   "vehicleIncluded": true,
        ///   "authorizedTime": "02:00:00", // Tiempo autorizado actualizado
        ///   "licensePlate": "XYZA-34",    // Patente actualizada
        ///   "idParkingSpot": 7,          // Estacionamiento actualizado
        ///   "idVisitType": 4             // Tipo de visita actualizado (ej., "Técnico")
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">El **ID** del registro de visita que se desea actualizar.</param>
        /// <param name="dto">
        /// **Objeto DTO con los datos actualizados para la visita.**
        /// Se espera un objeto JSON con las mismas propiedades que <see cref="VisitDto"/> utilizado para la creación,
        /// pero solo las propiedades `IdPerson`, `ZoneId`, `IdDirection`, `IdApartment`, `VehicleIncluded`,
        /// `AuthorizedTime`, `LicensePlate`, `IdParkingSpot`, y `IdVisitType` son directamente actualizables
        /// en el objeto `Visit` existente. El `EstablishmentId` en el DTO es para fines de autorización de `SUPERADMIN`.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="Visit"/> actualizado y 'Message' es "Visita actualizada correctamente.".</response>
        /// <response code="400">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "Debes especificar el ID del establecimiento." (para SUPERADMIN si no se proporciona).
        /// - "Debes especificar un estacionamiento si el vehículo está incluido."
        /// </response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "No se encontró el claim de establecimiento." (para ADMIN si no tiene).
        /// - "Rol no autorizado para editar visitas."
        /// - "No tienes permiso para editar esta visita." (si el usuario no tiene permisos sobre la visita encontrada).
        /// - "No puedes reasignar la visita a otro establecimiento." (si un ADMIN intenta cambiar el `EstablishmentId`).
        /// - "El tipo de visita no pertenece al establecimiento o no tienes acceso."
        /// - "La zona no pertenece al establecimiento o no tienes acceso."
        /// - "No tienes acceso a la subzona especificada."
        /// - "No tienes acceso al estacionamiento especificado."
        /// </response>
        /// <response code="404">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "Visita no encontrada."
        /// - "El establecimiento especificado no existe."
        /// - "El tipo de visita especificado no existe."
        /// - "La zona especificada no existe."
        /// - "El departamento especificado no existe o no está asociado a la zona indicada."
        /// - "El estacionamiento especificado no existe."
        /// </response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al actualizar la visita." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Actualización de información de visita")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateVisit(int id, [FromBody] VisitDto dto)
        {
            var response = await _visitService.UpdateVisitAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Elimina un registro de visita del sistema de forma permanente por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// **Reglas de eliminación:**
        /// - Solo los usuarios con permisos para acceder a la visita (`_userContext.CanAccessVisit`) pueden eliminarla. Esto significa que un `ADMIN` solo puede eliminar visitas de su propio establecimiento.
        /// - La eliminación es **física**, lo que implica que el registro de la visita será removido permanentemente de la base de datos.
        /// - La acción de eliminación se registra en el log de auditoría.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) del registro de visita a eliminar.</param>
        /// <returns>Un <see cref="ActionResult"/> de tipo <see cref="ResponseDto"/> que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Visita eliminada correctamente." si la operación fue exitosa.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permiso para eliminar esta visita." si el usuario no tiene los permisos necesarios sobre la visita.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Visita no encontrada." si el ID proporcionado no corresponde a un registro de visita existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al eliminar la visita." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Eliminación de visita")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVisit(int id)
        {
            var response = await _visitService.DeleteVisitAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}