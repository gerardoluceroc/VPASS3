using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.DTOs.PackagesDtos;
using VPASS3_backend.Models;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _pkgService;
        private readonly IUserContextService _userContext;

        public PackageController(IPackageService pkgService, IUserContextService userContext)
        {
            _pkgService = pkgService;
            _userContext = userContext;
        }

        /// <summary>
        /// Registra una nueva encomienda o paquete recibido en el establecimiento.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// El usuario debe tener permisos para registrar paquetes en el establecimiento asociado al departamento de destino.
        ///
        /// **Lógica de registro:**
        /// - Se valida que el `IdApartment` exista y que tenga una zona asociada.
        /// - Se verifica que el `IdApartmentOwnership` corresponda a un propietario activo y válido para el departamento especificado.
        /// - El `Recipient` es el nombre de la persona a quien va dirigido el paquete, tal como aparece en la encomienda.
        /// - `ReceivedAt` se establece automáticamente con la hora actual de Santiago de Chile al momento de la creación.
        /// - Si se proporciona un `IdPersonWhoReceived` en el DTO, se asume que el paquete fue entregado inmediatamente,
        ///   y `DeliveredAt` se establecerá automáticamente con la hora actual de Santiago de Chile. En caso contrario, `DeliveredAt` será nulo.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para CreatePackageDto (Paquete recibido en conserjería):**
        /// ```json
        /// {
        ///   "idApartment": 10,
        ///   "idApartmentOwnership": 5,
        ///   "recipient": "Juan Pérez",
        ///   "code": "AMZ123456" // Opcional
        ///   // "idPersonWhoReceived" se omite si no se entrega al instante
        /// }
        /// ```
        /// **Ejemplo de cuerpo de petición (Request Body) para CreatePackageDto (Paquete entregado al instante):**
        /// ```json
        /// {
        ///   "idApartment": 10,
        ///   "idApartmentOwnership": 5,
        ///   "recipient": "Juan Pérez",
        ///   "code": "AMZ123456", // Opcional
        ///   "idPersonWhoReceived": 15 // ID de la persona que retira el paquete al instante
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para el registro de la encomienda.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`IdApartment` (int, requerido):** El ID del departamento de destino del paquete.
        /// - **`IdApartmentOwnership` (int, requerido):** El ID del registro de propiedad que representa al inquilino activo en el departamento al momento de la recepción.
        /// - **`Recipient` (string, requerido):** El nombre del destinatario del paquete.
        /// - **`Code` (string?, opcional):** Un código de seguimiento o de la encomienda.
        /// - **`IdPersonWhoReceived` (int?, opcional):** El ID de la persona que retira el paquete. Solo se usa si el paquete es entregado inmediatamente tras su recepción.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="201">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="Package"/> creado y 'Message' es "Encomienda registrada correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos." si el DTO no es válido.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para registrar paquetes en este establecimiento." si el usuario no tiene los permisos para el establecimiento del departamento.</response>
        /// <response code="404">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "Departamento no encontrado." si el `IdApartment` no existe.
        /// - "Zona no encontrada para el departamento." si el departamento existe pero no tiene una zona asociada.
        /// - "No se ha encontrado el propietario para este departamento." si el `IdApartmentOwnership` no existe o no corresponde al departamento.
        /// - "No existen propietarios activos en el departamento" si no hay un `ApartmentOwnership` activo para el departamento.
        /// - "Persona que retira no encontrada." si se proporciona `IdPersonWhoReceived` y la persona no existe.
        /// </response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "El propietario ingresado ya no se encuentra viviendo en el departamento." si el `IdApartmentOwnership` proporcionado no es el propietario activo actual del departamento.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al registrar la encomienda." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreatePackageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));
            var resp = await _pkgService.CreateAsync(dto);
            return StatusCode(resp.StatusCode, resp);
        }


        /// <summary>
        /// Exporta los registros de encomiendas dentro de un rango de fechas específico a un archivo Excel (.xlsx).
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Los usuarios con rol **SUPERADMIN** pueden exportar encomiendas de todos los establecimientos dentro del rango de fechas.
        /// - Los usuarios con rol **ADMIN** solo podrán exportar encomiendas de su propio establecimiento que se encuentren dentro del rango de fechas.
        ///
        /// El archivo Excel incluirá las mismas columnas que la exportación total:
        /// "ID", "Código", "Destinatario", "Fecha de Recepción", "Destino", "Inquilino al Momento", "Fecha de Entrega", "Persona que Retiró".
        /// Los datos se filtrarán por la fecha de recepción (`ReceivedAt`) dentro del rango especificado (`StartDate` a `EndDate`).
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para GetPackagesByDatesDto:**
        /// ```json
        /// {
        ///   "startDate": "2024-01-01",
        ///   "endDate": "2024-01-31"
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con las fechas de inicio y fin para el filtro de exportación.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`StartDate` (DateOnly, requerido):** La fecha de inicio del rango (inclusive), en formato "YYYY-MM-DD".
        /// - **`EndDate` (DateOnly, requerido):** La fecha de fin del rango (inclusive), en formato "YYYY-MM-DD".
        /// </param>
        /// <returns>Un <see cref="FileResult"/> que representa el archivo Excel para descargar.</returns>
        /// <response code="200">Retorna un archivo Excel (.xlsx) con los datos de las encomiendas filtradas.</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Parámetros inválidos." si el DTO no es válido, o "La fecha de inicio no puede ser posterior a la final." si las fechas están mal.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "Establecimiento no asociado." si el usuario no es SUPERADMIN y no tiene un ID de establecimiento asignado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al generar el archivo Excel." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("export/excel/byDates")]
        public async Task<IActionResult> ExportByDates([FromBody] GetPackagesByDatesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Parámetros inválidos."));

            var response = await _pkgService.ExportPackagesToExcelByDatesAsync(dto);
            if (response.StatusCode != 200) return StatusCode(response.StatusCode, response);

            var file = (dynamic)response.Data!;
            return File((byte[])file.FileContent, file.ContentType, file.FileName);
        }

        /// <summary>
        /// Obtiene una lista de todas las encomiendas o paquetes registrados en el sistema,
        /// filtrados según los permisos del usuario autenticado.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Si el usuario es **SUPERADMIN**, la respuesta incluirá todos los paquetes de todos los establecimientos.
        /// - Si el usuario es **ADMIN**, solo se devolverán las encomiendas que pertenecen al establecimiento asociado a su cuenta.
        ///
        /// Cada paquete en la lista contendrá sus relaciones cargadas: el <see cref="Apartment"/> de destino (incluyendo su <see cref="Zone"/>),
        /// el registro de propiedad (<see cref="ApartmentOwnership"/>) con los datos de la <see cref="Person"/> asociada, y la <see cref="Person"/> que lo <see cref="Receiver"/> (si aplica).
        /// </remarks>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene una lista de objetos <see cref="Package"/> y 'Message' es "Encomiendas obtenidas correctamente.".</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes un establecimiento asociado." si el usuario no es SUPERADMIN y no tiene un ID de establecimiento asignado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener las encomiendas." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var resp = await _pkgService.GetAllAsync();
            return StatusCode(resp.StatusCode, resp);
        }

        /// <summary>
        /// Obtiene los detalles de una encomienda o paquete específico por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Los usuarios con rol **SUPERADMIN** pueden acceder a cualquier encomienda por su ID.
        /// - Los usuarios con rol **ADMIN** solo pueden ver las encomiendas que pertenezcan al establecimiento asociado a su cuenta.
        ///
        /// La respuesta incluirá los detalles completos del paquete, incluyendo:
        /// - El <see cref="Apartment"/> de destino y su <see cref="Zone"/>.
        /// - La información del propietario del apartamento (<see cref="ApartmentOwnership"/>) y la <see cref="Person"/> asociada.
        /// - Los detalles de la <see cref="Person"/> que recibió el paquete (<see cref="Receiver"/>), si ya fue entregado.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) de la encomienda a recuperar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="Package"/> completo con todas sus relaciones cargadas.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para ver esta encomienda." si el usuario no tiene los permisos para el establecimiento del paquete.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Encomienda no encontrada." si el ID proporcionado no corresponde a una encomienda existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al obtener la encomienda." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var resp = await _pkgService.GetByIdAsync(id);
            return StatusCode(resp.StatusCode, resp);
        }

        /// <summary>
        /// Exporta todos los registros de encomiendas a un archivo Excel (.xlsx).
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Si el usuario es **SUPERADMIN**, el archivo Excel contendrá todas las encomiendas de todos los establecimientos.
        /// - Si el usuario es **ADMIN**, el archivo Excel solo incluirá las encomiendas del establecimiento asociado a su cuenta.
        ///
        /// El archivo Excel generado incluirá las siguientes columnas:
        /// "ID", "Código", "Destinatario", "Fecha de Recepción", "Destino", "Inquilino al Momento", "Fecha de Entrega", "Persona que Retiró".
        /// Los campos de fecha y hora se formatearán como "dd/MM/yyyy HH:mm".
        /// </remarks>
        /// <returns>Un <see cref="FileResult"/> que representa el archivo Excel para descargar.</returns>
        /// <response code="200">Retorna un archivo Excel (.xlsx) con los datos de las encomiendas.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes un establecimiento asociado." si el usuario no es SUPERADMIN y no tiene un ID de establecimiento asignado.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error al generar el archivo de encomiendas." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("export/excel/all")]
        public async Task<IActionResult> ExportAll()
        {
            var response = await _pkgService.ExportAllPackagesToExcelAsync();

            if (response.StatusCode != 200)
                return StatusCode(response.StatusCode, response);

            var file = (dynamic)response.Data!;
            return File((byte[])file.FileContent, file.ContentType, file.FileName);
        }

        /// <summary>
        /// Marca un paquete como entregado al destinatario, registrando la fecha y hora de entrega.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// El usuario debe tener permisos para registrar la entrega de paquetes en el establecimiento donde se encuentra el paquete.
        ///
        /// **Lógica de entrega:**
        /// - Se busca el paquete por su `IdPackage`. Si no existe o ya ha sido marcado como entregado, se retorna el error correspondiente.
        /// - Si se proporciona un `IdPersonWhoReceived` en el DTO, se valida que esa persona exista y se asocia al paquete como la persona que lo retiró.
        /// - `DeliveredAt` se establece automáticamente con la **hora actual de Santiago de Chile**, registrando el momento exacto de la entrega.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para ReceivePackageDto:**
        /// ```json
        /// {
        ///   "idPackage": 123, // ID del paquete a marcar como entregado
        ///   "idPersonWhoReceived": 45 // ID de la persona que retira el paquete (opcional, si se conoce su ID)
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para marcar la encomienda como entregada.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`IdPackage` (int, requerido):** El ID único del paquete que se va a marcar como entregado.
        /// - **`IdPersonWhoReceived` (int?, opcional):** El ID de la persona que retira el paquete. Si se conoce la persona y ya existe en el sistema, se puede asociar directamente.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="Package"/> actualizado y 'Message' es "Entrega registrada correctamente.".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos." si el DTO no es válido.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para registrar la entrega de este paquete." si el usuario no tiene los permisos para el establecimiento del paquete.</response>
        /// <response code="404">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "Paquete no encontrado." si el `IdPackage` proporcionado no existe.
        /// - "Persona que retira no encontrada." si se proporciona `IdPersonWhoReceived` y la persona no existe.
        /// </response>
        /// <response code="409">Retorna un ResponseDto con 'Message' indicando "La encomienda ya fue marcada como entregada." si se intenta marcar como entregado un paquete que ya lo está.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al registrar la entrega." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("receive")]
        public async Task<ActionResult<ResponseDto>> ReceivePackage([FromBody] ReceivePackageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));

            var resp = await _pkgService.MarkAsDeliveredAsync(dto);
            return StatusCode(resp.StatusCode, resp);
        }

        /// <summary>
        /// Actualiza la información de un paquete o encomienda existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        /// El usuario debe tener permisos para actualizar paquetes en el establecimiento asociado.
        ///
        /// **Campos actualizables:**
        /// - **`Recipient` (Nombre del Destinatario):** Puede modificarse si el nombre original en la etiqueta era incorrecto o requiere ajuste.
        /// - **`Code` (Código de la encomienda):** Se puede actualizar si se omite o se modifica.
        /// - **`DeliveredAt` (Fecha de Entrega):** Puede establecerse manualmente si la entrega no se registró automáticamente o necesita corrección.
        /// - **`IdPersonWhoReceived` (Persona que Retiró):** Puede asociarse o modificarse con el ID de la persona que retiró el paquete. Si se proporciona, se valida que la persona exista.
        ///
        /// **Nota:** Si se intenta actualizar el `DeliveredAt` o `IdPersonWhoReceived` de un paquete que ya fue entregado, esta operación permitirá sobrescribir dichos valores.
        ///
        /// **Ejemplo de cuerpo de petición (Request Body) para UpdatePackageDto:**
        /// ```json
        /// {
        ///   "id": 123, // ID del paquete a actualizar (requerido)
        ///   "recipient": "Juan Pérez Actualizado", // Cambiar el nombre del destinatario
        ///   "code": "AMZ987654", // Cambiar o añadir el código
        ///   "deliveredAt": "2025-07-14T10:30:00", // Marcar como entregado a una hora específica
        ///   "idPersonWhoReceived": 45 // Asociar la persona que lo retiró
        /// }
        /// ```
        /// </remarks>
        /// <param name="dto">
        /// **Objeto DTO con los datos para actualizar el paquete.**
        /// Se espera un objeto JSON con las siguientes propiedades:
        /// - **`Id` (int, requerido):** El ID único del paquete a actualizar.
        /// - **`Recipient` (string?, opcional):** El nuevo nombre del destinatario. Si es nulo, no se actualiza.
        /// - **`Code` (string?, opcional):** El nuevo código de la encomienda. Si es nulo, no se actualiza.
        /// - **`DeliveredAt` (DateTime?, opcional):** La nueva fecha y hora de entrega. Si es nulo, no se actualiza.
        /// - **`IdPersonWhoReceived` (int?, opcional):** El ID de la persona que retira el paquete. Si es nulo, no se actualiza.
        /// </param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto donde 'Data' contiene el objeto <see cref="Package"/> actualizado y 'Message' es "Encomienda marcada como entregada exitósamente".</response>
        /// <response code="400">Retorna un ResponseDto con 'Message' indicando "Datos inválidos." si el DTO no es válido.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para actualizar este paquete." si el usuario no tiene los permisos para el establecimiento del paquete.</response>
        /// <response code="404">
        /// Retorna un ResponseDto con un mensaje de error si:
        /// - "Paquete no encontrado." si el `Id` proporcionado no existe.
        /// - "Persona que retira no encontrada." si se proporciona `IdPersonWhoReceived` y la persona no existe.
        /// </response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al actualizar la encomienda." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update")]
        public async Task<ActionResult<ResponseDto>> Update([FromBody] UpdatePackageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));
            var resp = await _pkgService.UpdateAsync(dto);
            return StatusCode(resp.StatusCode, resp);
        }

        /// <summary>
        /// Elimina una encomienda o paquete de forma física por su ID.
        /// </summary>
        /// <remarks>
        /// Este endpoint requiere **autenticación** y que el usuario tenga el rol **ADMIN** o **SUPERADMIN** (política "ManageOwnProfile").
        ///
        /// - Los usuarios con rol **SUPERADMIN** pueden eliminar cualquier encomienda por su ID.
        /// - Los usuarios con rol **ADMIN** solo pueden eliminar las encomiendas que pertenezcan al establecimiento asociado a su cuenta.
        ///
        /// La eliminación es **física**, lo que significa que el registro del paquete será removido permanentemente de la base de datos.
        /// Se incluye un registro en el log de auditoría detallando la eliminación.
        /// </remarks>
        /// <param name="id">El identificador único (ID entero) de la encomienda a eliminar.</param>
        /// <returns>Un ActionResult de tipo ResponseDto que representa el resultado de la operación.</returns>
        /// <response code="200">Retorna un ResponseDto con 'Message' indicando "Paquete eliminado correctamente." si la operación fue exitosa.</response>
        /// <response code="401">Retorna un ResponseDto con un mensaje de error si el usuario no está autenticado.</response>
        /// <response code="403">Retorna un ResponseDto con 'Message' indicando "No tienes permisos para eliminar esta encomienda." si el usuario no tiene los permisos para el establecimiento del paquete.</response>
        /// <response code="404">Retorna un ResponseDto con 'Message' indicando "Encomienda no encontrada." si el ID proporcionado no corresponde a una encomienda existente.</response>
        /// <response code="500">Retorna un ResponseDto con 'Message' indicando "Error en el servidor al eliminar el registro." si ocurre un error interno.</response>
        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var resp = await _pkgService.DeleteAsync(id);
            return StatusCode(resp.StatusCode, resp);
        }
    }
}