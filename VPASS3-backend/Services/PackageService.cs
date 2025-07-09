using VPASS3_backend.Context;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Utils;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Models;
using VPASS3_backend.DTOs.PackagesDtos;
using ClosedXML.Excel;
namespace VPASS3_backend.Services
{
    public class PackageService : IPackageService
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserContextService _userContext;
        private readonly IAuditLogService _auditLog;

        public PackageService(AppDbContext dbContext, IUserContextService userContext, IAuditLogService auditLog)
        {
            _dbContext = dbContext;
            _userContext = userContext;
            _auditLog = auditLog;
        }

        public async Task<ResponseDto> CreateAsync(CreatePackageDto dto)
        {
            // Se verifica que el departamento exista y tenga zona asociada
            var apartment = await _dbContext.Apartments
                .Include(a => a.Zone)
                .FirstOrDefaultAsync(a => a.Id == dto.IdApartment);

            if (apartment == null)
                return new ResponseDto(404, message: "Departamento no encontrado.");

            if (apartment.Zone == null)
                return new ResponseDto(404, message: "Zona no encontrada para el departamento.");

            // Se verifica permiso por establecimiento si no es SUPERADMIN
            if (_userContext.UserRole != "SUPERADMIN" &&
                (!_userContext.EstablishmentId.HasValue || apartment.Zone.EstablishmentId != _userContext.EstablishmentId.Value))
            {
                return new ResponseDto(403, message: "No tienes permisos para registrar paquetes en este establecimiento.");
            }

            // Se verifica que el registro de propiedad enviado exista
            var ownership = await _dbContext.ApartmentOwnerships
                .Include(o => o.Person)
                .FirstOrDefaultAsync(o => o.Id == dto.IdApartmentOwnership && o.IdApartment == dto.IdApartment);

            if (ownership == null)
                return new ResponseDto(404, message: "No se ha encontrado el propietario para este departamento.");

            var ultimateOwnership = await ApartmentUtils.GetActiveOwnershipAsync(_dbContext, dto.IdApartment);

            if (ultimateOwnership == null)
                return new ResponseDto(404, message: "No existen propietarios activos en el departamento");

            if (ultimateOwnership.Id != dto.IdApartmentOwnership)
                return new ResponseDto(409, message: "El propietario ingresado ya no se encuentra viviendo en el departamento.");

            // Si viene un receptor, se verifica que exista
            Person? receiver = null;
            DateTime? deliveredAt = null;

            if (dto.IdPersonWhoReceived.HasValue)
            {
                receiver = await _dbContext.Persons.FindAsync(dto.IdPersonWhoReceived.Value);
                if (receiver == null)
                    return new ResponseDto(404, message: "Persona que retira no encontrada.");

                // Si el receptor existe, se asume que el paquete fue entregado en el mismo momento
                deliveredAt = TimeHelper.GetSantiagoTime();
            }

            // Crear paquete
            var package = new Package
            {
                IdApartment = dto.IdApartment,
                IdApartmentOwnership = dto.IdApartmentOwnership,
                Code = dto.Code,
                IdPersonWhoReceived = dto.IdPersonWhoReceived,
                ReceivedAt = TimeHelper.GetSantiagoTime(),
                DeliveredAt = deliveredAt,
                Recipient = dto.Recipient,
            };

            _dbContext.Packages.Add(package);
            await _dbContext.SaveChangesAsync();

            // Se registra en log de auditoría
            await _auditLog.LogManualAsync(
                action: $"Se registró paquete para el dpto {apartment.Name ?? $"ID {apartment.Id}"}, propietario {ownership.Person.Names} {ownership.Person.LastNames}",
                email: _userContext.UserEmail,
                role: _userContext.UserRole,
                userId: _userContext.UserId ?? 0,
                endpoint: "/package/create",
                httpMethod: "POST",
                statusCode: 201
            );
            return new ResponseDto(201, package, "Encomienda registrada correctamente.");
        }

        public async Task<ResponseDto> ExportPackagesToExcelByDatesAsync(GetPackagesByDatesDto dto)
        {
            try
            {
                if (dto.StartDate > dto.EndDate)
                    return new ResponseDto(400, message: "La fecha de inicio no puede ser posterior a la final.");

                var startDate = dto.StartDate.ToDateTime(TimeOnly.MinValue);
                var endDate = dto.EndDate.ToDateTime(TimeOnly.MaxValue);

                var query = _dbContext.Packages
                    .Include(p => p.Apartment).ThenInclude(a => a.Zone)
                    .Include(p => p.Ownership).ThenInclude(o => o.Person)
                    .Include(p => p.Receiver)
                    .Where(p => p.ReceivedAt >= startDate && p.ReceivedAt <= endDate);

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "Establecimiento no asociado.");
                    query = query.Where(p => p.Apartment.Zone.EstablishmentId == _userContext.EstablishmentId.Value);
                }

                var packages = await query.OrderByDescending(p => p.ReceivedAt).ToListAsync();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Encomiendas");

                var headers = new[] {
                    "ID", "Código", "Destinatario", "Fecha de Recepción",
                    "Destino", "Inquilino al Momento", "Fecha de Entrega", "Persona que Retiró"
                };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                    worksheet.Cell(1, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                if (packages.Any())
                {
                    int row = 2;
                    foreach (var pkg in packages)
                    {
                        var depto = $"{pkg.Apartment.Zone?.Name} - Depto {pkg.Apartment?.Name}";
                        var owner = $"{pkg.Ownership?.Person?.Names} {pkg.Ownership?.Person?.LastNames}";
                        var recipient = pkg.Recipient;

                        // Evaluar si existe persona que retira
                        var personWhoReceived = pkg.Receiver != null
                            ? $"{pkg.Receiver.Names} {pkg.Receiver.LastNames}"
                            : "N/A";

                        worksheet.Cell(row, 1).Value = pkg.Id;
                        worksheet.Cell(row, 2).Value = pkg.Code ?? "Sin código";
                        worksheet.Cell(row, 3).Value = recipient ?? "Sin destinatario";
                        worksheet.Cell(row, 4).Value = pkg.ReceivedAt;
                        worksheet.Cell(row, 5).Value = depto;
                        worksheet.Cell(row, 6).Value = owner;
                        worksheet.Cell(row, 7).Value = pkg.DeliveredAt?.ToString("dd/MM/yyyy HH:mm") ?? "Pendiente";
                        worksheet.Cell(row, 8).Value = personWhoReceived;

                        worksheet.Cell(row, 4).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                        row++;
                    }
                }
                else
                {
                    worksheet.Cell(2, 1).Value = "No se encontraron encomiendas en el rango indicado.";
                    worksheet.Range(2, 1, 2, headers.Length).Merge();
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                string estName = _userContext.UserRole == "SUPERADMIN"
                    ? "Todos_los_Establecimientos"
                    : (await _dbContext.Establishments.FindAsync(_userContext.EstablishmentId))?.Name ?? "Indeterminado";

                string fileName = $"Encomiendas_{estName}_{dto.StartDate:yyyyMMdd}_al_{dto.EndDate:yyyyMMdd}.xlsx";

                await _auditLog.LogManualAsync(
                    action: $"Se ha descargado el reporte de las encomiendas del establecimiento {estName} entre el {dto.StartDate:dd/MM/yyyy} y {dto.EndDate:dd/MM/yyyy}",
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                userId: _userContext.UserId ?? 0,
                    endpoint: "/package/export/excel/byDates",
                    httpMethod: "POST",
                    statusCode: 200
                );

                return new ResponseDto(200, new
                {
                    FileContent = content,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = fileName
                }, "Archivo generado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al exportar Excel de encomiendas: {ex.Message}");
                return new ResponseDto(500, message: "Error al generar el archivo Excel.");
            }
        }

        public async Task<ResponseDto> ExportAllPackagesToExcelAsync()
        {
            try
            {
                var query = _dbContext.Packages
                    .Include(p => p.Apartment)
                        .ThenInclude(a => a.Zone)
                    .Include(p => p.Ownership)
                        .ThenInclude(o => o.Person)
                    .Include(p => p.Receiver)
                    .AsQueryable();

                // Si no es SUPERADMIN, se filtra por establecimiento
                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    query = query.Where(p => p.Apartment.Zone.EstablishmentId == _userContext.EstablishmentId.Value);
                }

                var packages = await query.OrderByDescending(p => p.ReceivedAt).ToListAsync();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Encomiendas");

                var headers = new[] {
                    "ID", "Código", "Destinatario", "Fecha de Recepción",
                    "Destino", "Inquilino al Momento", "Fecha de Entrega", "Persona que Retiró"
                };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                    worksheet.Cell(1, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                if (packages.Any())
                {
                    int row = 2;
                    foreach (var pkg in packages)
                    {
                        var depto = $"{pkg.Apartment.Zone?.Name} - Depto {pkg.Apartment?.Name}";
                        var owner = $"{pkg.Ownership?.Person?.Names} {pkg.Ownership?.Person?.LastNames}";
                        var recipient = pkg.Recipient;

                        // Evaluar si existe persona que retira
                        var personWhoReceived = pkg.Receiver != null
                            ? $"{pkg.Receiver.Names} {pkg.Receiver.LastNames}"
                            : "N/A";

                        worksheet.Cell(row, 1).Value = pkg.Id;
                        worksheet.Cell(row, 2).Value = pkg.Code ?? "Sin código";
                        worksheet.Cell(row, 3).Value = recipient ?? "Sin destinatario";
                        worksheet.Cell(row, 4).Value = pkg.ReceivedAt;
                        worksheet.Cell(row, 5).Value = depto;
                        worksheet.Cell(row, 6).Value = owner;
                        worksheet.Cell(row, 7).Value = pkg.DeliveredAt?.ToString("dd/MM/yyyy HH:mm") ?? "Pendiente";
                        worksheet.Cell(row, 8).Value = personWhoReceived;

                        worksheet.Cell(row, 4).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                        row++;
                    }
                }
                else
                {
                    worksheet.Cell(2, 1).Value = "No se encontraron encomiendas en el rango indicado.";
                    worksheet.Range(2, 1, 2, headers.Length).Merge();
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                var name = _userContext.UserRole == "SUPERADMIN"
                    ? "Todos_los_Establecimientos"
                    : (await _dbContext.Establishments.FirstOrDefaultAsync(e => e.Id == _userContext.EstablishmentId))?.Name ?? "Establecimiento";

                var fileName = $"Encomiendas_{name}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

                await _auditLog.LogManualAsync(
                    action: $"Se ha descargado el reporte de todas las encomiendas del establecimiento {name}.",
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: "/package/export/excel/all",
                    httpMethod: "GET",
                    statusCode: 200
                );

                return new ResponseDto(200, new
                {
                    FileContent = content,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = fileName
                }, "Archivo generado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ExportAllPackagesToExcelAsync: {ex.Message}");
                return new ResponseDto(500, message: "Error al generar el archivo de encomiendas.");
            }
        }

        public async Task<ResponseDto> MarkAsDeliveredAsync(ReceivePackageDto dto)
        {
            // Obtener paquete con todas sus relaciones necesarias
            var pkg = await _dbContext.Packages
                .Include(p => p.Apartment).ThenInclude(a => a.Zone)
                .Include(p => p.Ownership).ThenInclude(o => o.Person)
                .FirstOrDefaultAsync(p => p.Id == dto.IdPackage);

            if (pkg == null)
                return new ResponseDto(404, message: "Paquete no encontrado.");

            if (_userContext.UserRole != "SUPERADMIN" &&
                (!_userContext.EstablishmentId.HasValue || pkg.Apartment.Zone.EstablishmentId != _userContext.EstablishmentId.Value))
            {
                return new ResponseDto(403, message: "No tienes permisos para registrar la entrega de este paquete.");
            }

            if (pkg.DeliveredAt != null)
                return new ResponseDto(409, message: "La encomienda ya fue marcada como entregada.");

            // Validar persona si se especifica
            if (dto.IdPersonWhoReceived.HasValue)
            {
                var receiver = await _dbContext.Persons.FindAsync(dto.IdPersonWhoReceived.Value);
                if (receiver == null)
                    return new ResponseDto(404, message: "Persona que retira no encontrada.");
                pkg.IdPersonWhoReceived = dto.IdPersonWhoReceived;
            }

            pkg.DeliveredAt = TimeHelper.GetSantiagoTime();
            await _dbContext.SaveChangesAsync();

            await _auditLog.LogManualAsync(
                action: $"Encomienda con ID {pkg.Id} entregada para el depto {pkg.Apartment.Name ?? $"ID {pkg.IdApartment}"}.",
                email: _userContext.UserEmail,
                role: _userContext.UserRole,
                userId: _userContext.UserId ?? 0,
                endpoint: "/package/receive",
                httpMethod: "PUT",
                statusCode: 200
            );
            return new ResponseDto(200, pkg, "Entrega registrada correctamente.");
        }

        public async Task<ResponseDto> UpdateAsync(UpdatePackageDto dto)
        {
            var pkg = await _dbContext.Packages
                .Include(p => p.Apartment).ThenInclude(a => a.Zone)
                .Include(p => p.Ownership).ThenInclude(o => o.Person)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);
            if (pkg == null) return new ResponseDto(404, message: "Paquete no encontrado.");

            if (_userContext.UserRole != "SUPERADMIN" &&
                (!_userContext.EstablishmentId.HasValue || pkg.Apartment.Zone.EstablishmentId != _userContext.EstablishmentId.Value))
                return new ResponseDto(403, message: "No tienes permisos para actualizar este paquete.");

            if (dto.Code != null) pkg.Code = dto.Code;
            if (dto.DeliveredAt.HasValue) pkg.DeliveredAt = dto.DeliveredAt;
            if (dto.IdPersonWhoReceived.HasValue)
            {
                var receiver = await _dbContext.Persons.FindAsync(dto.IdPersonWhoReceived.Value);
                if (receiver == null) return new ResponseDto(404, message: "Persona que retira no encontrada.");
                pkg.IdPersonWhoReceived = dto.IdPersonWhoReceived;
            }
            pkg.Recipient = dto.Recipient ?? pkg.Recipient; // Actualizar destinatario si se proporciona

            await _dbContext.SaveChangesAsync();

            await _auditLog.LogManualAsync(
                action: $"Se actualizó paquete ID {pkg.Id} para el departamento {pkg.Apartment.Name ?? $"ID {pkg.IdApartment}"}",
                email: _userContext.UserEmail,
                role: _userContext.UserRole,
                userId: _userContext.UserId ?? 0,
                endpoint: "/package/update",
                httpMethod: "PUT",
                statusCode: 200
            );

            return new ResponseDto(200, pkg, "Encomienda marcada como entregada exitósamente");
        }

        public async Task<ResponseDto> GetAllAsync()
        {
            var query = _dbContext.Packages
                .Include(p => p.Apartment).ThenInclude(a => a.Zone)
                .Include(p => p.Ownership).ThenInclude(o => o.Person)
                .Include(p => p.Receiver)
                .AsQueryable();

            if (_userContext.UserRole != "SUPERADMIN")
            {
                if (!_userContext.EstablishmentId.HasValue) return new ResponseDto(403, message: "No tienes un establecimiento asociado.");
                query = query.Where(p => p.Apartment.Zone.EstablishmentId == _userContext.EstablishmentId.Value);
            }

            var list = await query.ToListAsync();
            return new ResponseDto(200, list, "Encomiendas obtenidas correctamente.");
        }

        public async Task<ResponseDto> GetByIdAsync(int id)
        {
            var pkg = await _dbContext.Packages
                .Include(p => p.Apartment).ThenInclude(a => a.Zone)
                .Include(p => p.Ownership).ThenInclude(o => o.Person)
                .Include(p => p.Receiver)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pkg == null) return new ResponseDto(404, message: "Encomienda no encontrado.");

            if (_userContext.UserRole != "SUPERADMIN" &&
                (!_userContext.EstablishmentId.HasValue || pkg.Apartment.Zone.EstablishmentId != _userContext.EstablishmentId.Value))
                return new ResponseDto(403, message: "No tienes permisos para ver esta encomienda.");

            return new ResponseDto(200, pkg);
        }

        public async Task<ResponseDto> DeleteAsync(int id)
        {
            // Se obtiene el paquete con sus relaciones necesarias
            var pkg = await _dbContext.Packages
                .Include(p => p.Apartment).ThenInclude(a => a.Zone)
                .Include(p => p.Ownership).ThenInclude(o => o.Person)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pkg == null)
                return new ResponseDto(404, message: "Encomiendanno enconrrada.");

            // Validación de permisos por establecimiento
            if (_userContext.UserRole != "SUPERADMIN" &&
                (!_userContext.EstablishmentId.HasValue || pkg.Apartment.Zone.EstablishmentId != _userContext.EstablishmentId.Value))
            {
                return new ResponseDto(403, message: "No tienes permisos para eliminar esta enncomienda.");
            }

            _dbContext.Packages.Remove(pkg);
            await _dbContext.SaveChangesAsync();

            await _auditLog.LogManualAsync(
                action: $"Eliminó la encomienda ID {pkg.Id} del departamento {pkg.Apartment.Name ?? $"ID {pkg.Apartment.Id}"}",
                email: _userContext.UserEmail,
                role: _userContext.UserRole,
                userId: _userContext.UserId ?? 0,
                endpoint: "/package/delete/{id}",
                httpMethod: "DELETE",
                statusCode: 200
            );
            return new ResponseDto(200, message: "Paquete eliminado correctamente.");
        }
    }
}