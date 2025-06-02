using VPASS3_backend.Context;
using VPASS3_backend.DTOs.Visits;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Drawing;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace VPASS3_backend.Services
{
    public class VisitService : IVisitService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;
        private readonly IAuditLogService _auditLogService;

        public VisitService(AppDbContext context, IUserContextService userContext, IAuditLogService auditLogService)
        {
            _context = context;
            _userContext = userContext;
            _auditLogService = auditLogService;
        }


        public async Task<ResponseDto> GetAllVisitsAsync()
        {
            try
            {
                var visits = await _context.Visits
                    .Include(v => v.ParkingSpot)
                    .Include(v => v.VisitType)
                    .Include(v => v.Direction)
                    .Include(v => v.Zone)
                    .Include(v => v.Visitor)
                    .Include(v => v.ZoneSection)
                    .ToListAsync();

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    visits = visits
                        .Where(v => _userContext.CanAccessVisit(v))
                        .ToList();
                }

                // Limpia las subzonas en la zona principal de cada visita
                foreach (var visit in visits)
                {
                    if (visit.Zone != null && visit.Zone.ZoneSections != null)
                    {
                        visit.Zone.ZoneSections.Clear();
                    }
                }

                return new ResponseDto(200, visits, "Visitas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllVisitsAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener las visitas.");
            }
        }

        public async Task<ResponseDto> GetVisitByIdAsync(int id)
        {
            try
            {
                var visit = await _context.Visits
                    .Include(v => v.ParkingSpot)
                    .Include(v => v.VisitType)
                    .Include(v => v.Direction)
                    .Include(v => v.Zone)
                    .Include(v => v.Visitor)
                    .Include(v => v.ZoneSection)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (visit == null)
                    return new ResponseDto(404, message: "Visita no encontrada.");

                if (!_userContext.CanAccessVisit(visit))
                    return new ResponseDto(403, message: "No tienes permiso para acceder a esta visita.");

                // Limpiar las ZoneSections para evitar incluirlas en la respuesta
                if (visit.Zone != null && visit.Zone.ZoneSections != null)
                {
                    visit.Zone.ZoneSections.Clear();
                }

                return new ResponseDto(200, visit, "Visita obtenida correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetVisitByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener la visita.");
            }
        }

        public async Task<ResponseDto> CreateVisitAsync(VisitDto dto)
        {
            try
            {
                // Obtener el rol del usuario y el establecimiento directamente desde UserContext
                var userRole = _userContext.UserRole;
                int establishmentId;
                var direction = await _context.Directions.FindAsync(dto.IdDirection);

                if (userRole == "ADMIN")
                {
                    // Para ADMIN, el establecimiento está guardado en UserContext
                    if (_userContext.EstablishmentId == null)
                        return new ResponseDto(403, message: "No se encontró el establecimiento asociado al usuario.");

                    establishmentId = _userContext.EstablishmentId.Value;
                }
                else if (userRole == "SUPERADMIN")
                {
                    // Para SUPERADMIN, como no tiene un id de establecimiento especificado en su claim, se obtiene directamente del dto de entrada
                    if (!dto.EstablishmentId.HasValue)
                        return new ResponseDto(400, message: "Debes especificar el ID del establecimiento.");

                    establishmentId = dto.EstablishmentId.Value;
                }
                else
                {
                    return new ResponseDto(403, message: "Rol no autorizado para crear visitas.");
                }

                // Validar existencia del establecimiento
                var establishmentExists = await _context.Establishments.AnyAsync(e => e.Id == establishmentId);
                if (!establishmentExists)
                    return new ResponseDto(404, message: "El establecimiento especificado no existe.");

                // Validar tipo de visita
                var visitType = await _context.VisitTypes.FirstOrDefaultAsync(vt => vt.Id == dto.IdVisitType);
                if (visitType == null)
                    return new ResponseDto(404, message: "El tipo de visita especificado no existe.");

                if (!_userContext.CanAccessVisitType(visitType) || visitType.IdEstablishment != establishmentId)
                    return new ResponseDto(403, message: "No tienes acceso al tipo de visita o no corresponde al establecimiento.");

                // Validar zona
                var zone = await _context.Zones.FirstOrDefaultAsync(z => z.Id == dto.ZoneId);
                if (zone == null)
                    return new ResponseDto(404, message: "La zona especificada no existe.");

                if (!_userContext.CanAccessZone(zone) || zone.EstablishmentId != establishmentId)
                    return new ResponseDto(403, message: "No tienes acceso a la zona o no pertenece al establecimiento.");

                // Validar subzona si aplica
                int? idZoneSectionValida = null;
                if (dto.IdZoneSection.HasValue)
                {
                    var zoneSection = await _context.ZoneSections
                        .Include(zs => zs.Zone)
                        .FirstOrDefaultAsync(zs => zs.Id == dto.IdZoneSection.Value && zs.IdZone == dto.ZoneId);

                    if (zoneSection == null)
                        return new ResponseDto(404, message: "La subzona especificada no existe o no pertenece a la zona.");

                    if (!_userContext.CanAccessZoneSection(zoneSection))
                        return new ResponseDto(403, message: "No tienes acceso a la subzona especificada.");

                    idZoneSectionValida = zoneSection.Id;
                }

                // Validar estacionamiento si hay vehículo
                if (dto.VehicleIncluded)
                {
                    if (!dto.IdParkingSpot.HasValue)
                        return new ResponseDto(400, message: "Debes especificar un estacionamiento si el vehículo está incluido.");

                    var parkingSpot = await _context.ParkingSpots
                        .FirstOrDefaultAsync(p => p.Id == dto.IdParkingSpot.Value);

                    if (parkingSpot == null)
                        return new ResponseDto(404, message: "El estacionamiento especificado no existe.");

                    if (!_userContext.CanAccessParkingSpot(parkingSpot))
                        return new ResponseDto(403, message: "No tienes acceso al estacionamiento especificado.");

                    if (parkingSpot.IsAvailable.HasValue && !parkingSpot.IsAvailable.Value && (direction.VisitDirection.ToLower() == "entrada" || direction.Id == 1))
                    {
                        return new ResponseDto(400, message: "El estacionamiento seleccionado se encuentra ocupado.");
                    }
                }

                // Obtener hora local Chile
                TimeZoneInfo chileTimeZone;
                try
                {
                    chileTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time");
                }
                catch
                {
                    chileTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Santiago");
                }

                DateTime chileDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, chileTimeZone);

                // Crear la visita
                var visit = new Visit
                {
                    EstablishmentId = establishmentId,
                    VisitorId = dto.VisitorId,
                    ZoneId = dto.ZoneId,
                    IdDirection = dto.IdDirection,
                    VehicleIncluded = dto.VehicleIncluded,
                    LicensePlate = dto.VehicleIncluded ? dto.LicensePlate : null,
                    IdParkingSpot = dto.VehicleIncluded ? dto.IdParkingSpot : null,
                    EntryDate = chileDateTime,
                    IdZoneSection = idZoneSectionValida,
                    IdVisitType = dto.IdVisitType,
                    AuthorizedTime = dto.AuthorizedTime.HasValue ? dto.AuthorizedTime.Value : null
                };

                _context.Visits.Add(visit);

                // Si la visita incluye un estacionamiento
                if (dto.VehicleIncluded && dto.IdParkingSpot.HasValue)
                {
                    var parkingSpot = await _context.ParkingSpots.FindAsync(dto.IdParkingSpot.Value);

                    // Si la visita es de tipo salida
                    if (direction.VisitDirection.ToLower() == "salida" || direction.Id == 2)
                    {
                        // Se marca el estacionamiento como disponible ya que el visitante que lo ha utilizado se ha ido
                        if (parkingSpot != null)
                        {
                            parkingSpot.IsAvailable = true;
                        }
                    }

                    // Si la visita es de tipo entrada
                    else if (direction.VisitDirection.ToLower() == "entrada" || direction.Id == 1)
                    {
                        // Se marca el estacionamiento como no disponible ya que lo utilizará el visitante
                        if (parkingSpot != null)
                        {
                            parkingSpot.IsAvailable = false;
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return new ResponseDto(201, visit, "Visita creada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateVisitAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al crear la visita.");
            }
        }

        public async Task<ResponseDto> UpdateVisitAsync(int id, VisitDto dto)
        {
            try
            {
                int establishmentId;

                var userRole = _userContext.UserRole;

                if (userRole == "ADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No se encontró el claim de establecimiento.");

                    establishmentId = _userContext.EstablishmentId.Value;
                }
                else if (userRole == "SUPERADMIN")
                {
                    // Para SUPERADMIN, como no tiene un id de establecimiento especificado en su claim, se obtiene directamente del dto de entrada
                    if (!dto.EstablishmentId.HasValue)
                        return new ResponseDto(400, message: "Debes especificar el ID del establecimiento.");

                    establishmentId = dto.EstablishmentId.Value;
                }
                else
                {
                    return new ResponseDto(403, message: "Rol no autorizado para editar visitas.");
                }

                // Buscar la visita
                var visit = await _context.Visits.FirstOrDefaultAsync(v => v.Id == id);
                if (visit == null)
                    return new ResponseDto(404, message: "Visita no encontrada.");

                if (!_userContext.CanAccessVisit(visit))
                    return new ResponseDto(403, message: "No tienes permiso para editar esta visita.");

                if (!_userContext.CanAccessOwnEstablishment(establishmentId))
                    return new ResponseDto(403, message: "No puedes reasignar la visita a otro establecimiento.");

                // Verificar existencia del establecimiento
                var establishmentExists = await _context.Establishments.AnyAsync(e => e.Id == establishmentId);
                if (!establishmentExists)
                    return new ResponseDto(404, message: "El establecimiento especificado no existe.");

                // Verificar tipo de visita
                var visitType = await _context.VisitTypes.FirstOrDefaultAsync(vt => vt.Id == dto.IdVisitType);
                if (visitType == null)
                    return new ResponseDto(404, message: "El tipo de visita especificado no existe.");

                if (!_userContext.CanAccessVisitType(visitType) || visitType.IdEstablishment != establishmentId)
                    return new ResponseDto(403, message: "El tipo de visita no pertenece al establecimiento o no tienes acceso.");

                // Verificar zona
                var zone = await _context.Zones.FirstOrDefaultAsync(z => z.Id == dto.ZoneId);
                if (zone == null)
                    return new ResponseDto(404, message: "La zona especificada no existe.");

                if (!_userContext.CanAccessZone(zone) || zone.EstablishmentId != establishmentId)
                    return new ResponseDto(403, message: "La zona no pertenece al establecimiento o no tienes acceso.");

                // Verificar subzona
                int? idZoneSectionValida = null;
                if (dto.IdZoneSection.HasValue)
                {
                    var zoneSection = await _context.ZoneSections
                        .Include(zs => zs.Zone)
                        .FirstOrDefaultAsync(zs => zs.Id == dto.IdZoneSection.Value && zs.IdZone == dto.ZoneId);

                    if (zoneSection == null)
                        return new ResponseDto(404, message: "La subzona especificada no existe o no está asociada a la zona indicada.");

                    if (!_userContext.CanAccessZoneSection(zoneSection))
                        return new ResponseDto(403, message: "No tienes acceso a la subzona especificada.");

                    idZoneSectionValida = zoneSection.Id;
                }

                // Verificar estacionamiento si hay vehículo
                if (dto.VehicleIncluded)
                {
                    if (!dto.IdParkingSpot.HasValue)
                        return new ResponseDto(400, message: "Debes especificar un estacionamiento si el vehículo está incluido.");

                    var parkingSpot = await _context.ParkingSpots.FirstOrDefaultAsync(p => p.Id == dto.IdParkingSpot.Value);
                    if (parkingSpot == null)
                        return new ResponseDto(404, message: "El estacionamiento especificado no existe.");

                    if (!_userContext.CanAccessParkingSpot(parkingSpot))
                        return new ResponseDto(403, message: "No tienes acceso al estacionamiento especificado.");
                }

                // Asignar valores
                visit.EstablishmentId = establishmentId;
                visit.VisitorId = dto.VisitorId;
                visit.ZoneId = dto.ZoneId;
                visit.VehicleIncluded = dto.VehicleIncluded;
                visit.IdDirection = dto.IdDirection;
                visit.IdZoneSection = idZoneSectionValida;
                visit.LicensePlate = dto.VehicleIncluded ? dto.LicensePlate : null;
                visit.IdParkingSpot = dto.VehicleIncluded ? dto.IdParkingSpot : null;
                visit.IdVisitType = dto.IdVisitType;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, visit, "Visita actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateVisitAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al actualizar la visita.");
            }
        }


        public async Task<ResponseDto> DeleteVisitAsync(int id)
        {
            try
            {
                var visit = await _context.Visits
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (visit == null)
                    return new ResponseDto(404, message: "Visita no encontrada.");

                if (!_userContext.CanAccessVisit(visit))
                    return new ResponseDto(403, message: "No tienes permiso para eliminar esta visita.");

                _context.Visits.Remove(visit);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, message: "Visita eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteVisitAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar la visita.");
            }
        }

        public async Task<ResponseDto> ExportVisitsToExcelAsync(GetVisitByDatesDto dto)
        {
            try
            {
                // Validar fechas
                if (dto.StartDate > dto.EndDate)
                {
                    return new ResponseDto(400, message: "La fecha de inicio no puede ser posterior a la fecha final.");
                }

                // Convertir DateOnly a DateTime (inicio del día y fin del día)
                var startDate = dto.StartDate.ToDateTime(TimeOnly.MinValue);
                var endDate = dto.EndDate.ToDateTime(TimeOnly.MaxValue);

                // Obtener visitas según permisos
                var visitsQuery = _context.Visits
                    .Include(v => v.Visitor)
                    .Include(v => v.Zone)
                    .Include(v => v.ZoneSection)
                    .Include(v => v.VisitType)
                    .Include(v => v.Direction)
                    .Include(v => v.ParkingSpot)
                    .Include(v => v.Establishment)
                    .Where(v => v.EntryDate >= startDate && v.EntryDate <= endDate);

                // Filtrar por establecimiento si no es SUPERADMIN
                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    visitsQuery = visitsQuery.Where(v => v.EstablishmentId == _userContext.EstablishmentId.Value);
                }

                // Ordenar por fecha (más reciente primero)
                var visits = await visitsQuery
                    .OrderByDescending(v => v.EntryDate)
                    .ToListAsync();

                // Crear el archivo Excel
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Visitas");

                    // Encabezados
                    worksheet.Cell(1, 1).Value = "ID Visita";
                    worksheet.Cell(1, 2).Value = "Fecha y Hora";
                    worksheet.Cell(1, 3).Value = "Visitante";
                    worksheet.Cell(1, 4).Value = "Tipo de Visita";
                    worksheet.Cell(1, 5).Value = "Establecimiento";
                    worksheet.Cell(1, 6).Value = "Destino";
                    worksheet.Cell(1, 7).Value = "Dirección";
                    worksheet.Cell(1, 8).Value = "Vehículo";
                    worksheet.Cell(1, 9).Value = "Patente";
                    worksheet.Cell(1, 10).Value = "Estacionamiento";

                    // Estilo para los encabezados
                    var headerRange = worksheet.Range(1, 1, 1, 10);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Datos (solo si hay visitas)
                    if (visits.Any())
                    {
                        int row = 2;
                        foreach (var visit in visits)
                        {
                            worksheet.Cell(row, 1).Value = visit.Id;
                            worksheet.Cell(row, 2).Value = visit.EntryDate;
                            worksheet.Cell(row, 3).Value = $"{visit.Visitor?.Names} {visit.Visitor?.LastNames}";
                            worksheet.Cell(row, 4).Value = visit.VisitType?.Name;
                            worksheet.Cell(row, 5).Value = visit.Establishment?.Name;
                            worksheet.Cell(row, 6).Value = $"{visit.Zone?.Name} - {visit.ZoneSection?.Name}";
                            worksheet.Cell(row, 7).Value = visit.Direction?.VisitDirection;
                            worksheet.Cell(row, 8).Value = visit.VehicleIncluded ? "Sí" : "No";
                            worksheet.Cell(row, 9).Value = visit.LicensePlate ?? "N/A";
                            worksheet.Cell(row, 10).Value = visit.ParkingSpot?.Name ?? "N/A";

                            // Formato de fecha
                            worksheet.Cell(row, 2).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                            row++;
                        }
                    }
                    else
                    {
                        // Mensaje cuando no hay datos
                        worksheet.Cell(2, 1).Value = "No se encontraron visitas en el rango especificado";
                        worksheet.Range(2, 1, 2, 10).Merge();
                    }

                    // Autoajustar columnas
                    worksheet.Columns().AdjustToContents();

                    // Guardar en memoria
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        // Obtener nombre del archivo
                        string establishmentName;
                        if (_userContext.UserRole == "SUPERADMIN")
                        {
                            establishmentName = "Todos_los_Establecimientos";
                        }
                        else if (visits.Any())
                        {
                            establishmentName = visits.First().Establishment?.Name ?? "Indeterminado";
                        }
                        else
                        {
                            // Si no hay visitas, obtenemos el nombre del establecimiento del usuario
                            var establishment = await _context.Establishments
                                .FirstOrDefaultAsync(e => e.Id == _userContext.EstablishmentId);
                            establishmentName = establishment?.Name ?? "Indeterminado";
                        }

                        var fileName = $"Visitas_{establishmentName}_{dto.StartDate:yyyyMMdd}_al_{dto.EndDate:yyyyMMdd}.xlsx";

                        // Registro de auditoría
                        var message = _userContext.UserRole == "SUPERADMIN"
                            ? $"Se ha descargado el reporte de visitas de todos los establecimientos entre las fechas {dto.StartDate:dd/MM/yyyy} y {dto.EndDate:dd/MM/yyyy}"
                            : $"Se ha descargado el reporte de visitas del establecimiento {establishmentName} entre las fechas {dto.StartDate:dd/MM/yyyy} y {dto.EndDate:dd/MM/yyyy}";

                        await _auditLogService.LogManualAsync(
                            action: message,
                            email: _userContext.UserEmail,
                            role: _userContext.UserRole,
                            userId: _userContext.UserId ?? 0,
                            endpoint: "/Visit/export/excel/byDates",
                            httpMethod: "POST",
                            statusCode: 200
                        );

                        return new ResponseDto(200, new
                        {
                            FileContent = content,
                            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            FileName = fileName
                        }, visits.Any()
                            ? "Reporte generado correctamente."
                            : "Reporte generado sin datos para el rango especificado.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ExportVisitsToExcelAsync: {ex.Message}");
                return new ResponseDto(500, message: "Error al generar el reporte de visitas.");
            }
        }

        public async Task<ResponseDto> ExportVisitsToExcelByIdentificationNumberAsync(ExportVisitsExcelByIdentificationNumberDto dto)
        {
            try
            {
                // Validar que el visitante exista
                var visitor = await _context.Visitors
                    .FirstOrDefaultAsync(v => v.IdentificationNumber == dto.IdentificationNumber);

                if (visitor == null)
                {
                    return new ResponseDto(404, message: "Visitante no encontrado.");
                }

                // Obtener visitas según permisos
                var visitsQuery = _context.Visits
                    .Include(v => v.Visitor)
                    .Include(v => v.Zone)
                    .Include(v => v.ZoneSection)
                    .Include(v => v.VisitType)
                    .Include(v => v.Direction)
                    .Include(v => v.ParkingSpot)
                    .Include(v => v.Establishment)
                    .Where(v => v.VisitorId == visitor.Id);

                // Filtrar por establecimiento si no es SUPERADMIN
                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    visitsQuery = visitsQuery.Where(v => v.EstablishmentId == _userContext.EstablishmentId.Value);
                }

                // Ordenar por fecha (más reciente primero)
                var visits = await visitsQuery
                    .OrderByDescending(v => v.EntryDate)
                    .ToListAsync();

                // Crear el archivo Excel
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Visitas");

                    // Encabezados
                    worksheet.Cell(1, 1).Value = "ID Visita";
                    worksheet.Cell(1, 2).Value = "Fecha y Hora";
                    worksheet.Cell(1, 3).Value = "Visitante";
                    worksheet.Cell(1, 4).Value = "Tipo de Visita";
                    worksheet.Cell(1, 5).Value = "Establecimiento";
                    worksheet.Cell(1, 6).Value = "Destino";
                    worksheet.Cell(1, 7).Value = "Dirección";
                    worksheet.Cell(1, 8).Value = "Vehículo";
                    worksheet.Cell(1, 9).Value = "Patente";
                    worksheet.Cell(1, 10).Value = "Estacionamiento";

                    // Estilo para los encabezados
                    var headerRange = worksheet.Range(1, 1, 1, 10);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Datos (solo si hay visitas)
                    if (visits.Any())
                    {
                        int row = 2;
                        foreach (var visit in visits)
                        {
                            worksheet.Cell(row, 1).Value = visit.Id;
                            worksheet.Cell(row, 2).Value = visit.EntryDate;
                            worksheet.Cell(row, 3).Value = $"{visit.Visitor?.Names} {visit.Visitor?.LastNames}";
                            worksheet.Cell(row, 4).Value = visit.VisitType?.Name;
                            worksheet.Cell(row, 5).Value = visit.Establishment?.Name;
                            worksheet.Cell(row, 6).Value = $"{visit.Zone?.Name} - {visit.ZoneSection?.Name}";
                            worksheet.Cell(row, 7).Value = visit.Direction?.VisitDirection;
                            worksheet.Cell(row, 8).Value = visit.VehicleIncluded ? "Sí" : "No";
                            worksheet.Cell(row, 9).Value = visit.LicensePlate ?? "N/A";
                            worksheet.Cell(row, 10).Value = visit.ParkingSpot?.Name ?? "N/A";

                            // Formato de fecha
                            worksheet.Cell(row, 2).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                            row++;
                        }
                    }
                    else
                    {
                        // Mensaje cuando no hay datos
                        worksheet.Cell(2, 1).Value = $"No se encontraron visitas para {visitor.Names} {visitor.LastNames} ({visitor.IdentificationNumber})";
                        worksheet.Range(2, 1, 2, 10).Merge();
                    }

                    // Autoajustar columnas
                    worksheet.Columns().AdjustToContents();

                    // Guardar en memoria
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        // Obtener nombre del archivo
                        string establishmentName;
                        if (_userContext.UserRole == "SUPERADMIN")
                        {
                            establishmentName = "Todos_los_Establecimientos";
                        }
                        else if (visits.Any())
                        {
                            establishmentName = visits.First().Establishment?.Name ?? "Indeterminado";
                        }
                        else
                        {
                            // Si no hay visitas, obtenemos el nombre del establecimiento del usuario
                            var establishment = await _context.Establishments
                                .FirstOrDefaultAsync(e => e.Id == _userContext.EstablishmentId);
                            establishmentName = establishment?.Name ?? "Indeterminado";
                        }

                        // Obtener nombre del archivo
                        var fileName = $"Visitas_{visitor.Names}_{visitor.LastNames}_{visitor.IdentificationNumber}.xlsx";

                        // Mensaje para el log de auditoría
                        var message = _userContext.UserRole == "SUPERADMIN"
                            ? $"Se ha descargado el reporte de visitas de {visitor.Names} {visitor.LastNames} ({visitor.IdentificationNumber}) en todos los establecimientos"
                            : $"Se ha descargado el reporte de visitas de {visitor.Names} {visitor.LastNames} ({visitor.IdentificationNumber}) en el establecimiento {establishmentName}";

                        await _auditLogService.LogManualAsync(
                            action: message,
                            email: _userContext.UserEmail,
                            role: _userContext.UserRole,
                            userId: _userContext.UserId ?? 0,
                            endpoint: "/Visit/export/excel/byRut",
                            httpMethod: "POST",
                            statusCode: 200
                        );

                        return new ResponseDto(200, new
                        {
                            FileContent = content,
                            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            FileName = fileName
                        }, visits.Any()
                            ? $"Reporte de visitas generado para {visitor.Names} {visitor.LastNames} ({visitor.IdentificationNumber})"
                            : $"Reporte generado sin visitas para {visitor.Names} {visitor.LastNames} ({visitor.IdentificationNumber})");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ExportVisitsToExcelByIdentificationNumberAsync: {ex.Message}");
                return new ResponseDto(500, message: "Error al generar el reporte de visitas.");
            }
        }
    }
}