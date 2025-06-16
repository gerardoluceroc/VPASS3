using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Context;
using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.Enums;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Interfaces.CommonAreaInterfaces;
using VPASS3_backend.Models.CommonAreas;

namespace VPASS3_backend.Services.CommonAreaServices
{
    public class CommonAreaService : ICommonAreaService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;
        private readonly IAuditLogService _auditLogService;

        public CommonAreaService(AppDbContext context, IUserContextService userContext, IAuditLogService auditLogService)
        {
            _context = context;
            _userContext = userContext;
            _auditLogService = auditLogService;
        }
        public async Task<ResponseDto> GetAllAsync()
        {
            try
            {
                var query = _context.CommonAreas
                    .Include(ca => ca.Reservations)
                        .ThenInclude(r => r.ReservedBy)
                    .Include(ca => ca.Usages)
                        .ThenInclude(u => u.Person)
                    .AsQueryable();

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    query = query.Where(ca => ca.IdEstablishment == _userContext.EstablishmentId.Value);
                }

                var list = await query.ToListAsync();
                return new ResponseDto(200, list, "Áreas comunes obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetAll CommonAreaService: " + ex.Message);
                return new ResponseDto(500, message: "Error al obtener áreas comunes.");
            }
        }
        public async Task<ResponseDto> GetByIdAsync(int id)
        {
            try
            {
                var ca = await _context.CommonAreas
                    .Include(ca => ca.Reservations)
                        .ThenInclude(r => r.ReservedBy)
                    .Include(ca => ca.Usages)
                        .ThenInclude(u => u.Person)
                    .FirstOrDefaultAsync(e => e.Id == id);
                if (ca == null)
                    return new ResponseDto(404, message: "Área común no encontrada.");

                if (_userContext.UserRole != "SUPERADMIN" && ca.IdEstablishment != _userContext.EstablishmentId)
                    return new ResponseDto(403, message: "No tienes permisos para este recurso.");

                return new ResponseDto(200, ca, "Área común obtenida correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetById CommonAreaService: " + ex.Message);
                return new ResponseDto(500, message: "Error al obtener el área común.");
            }
        }
        public async Task<ResponseDto> CreateAsync(CreateCommonAreaDto dto)
        {
            try
            {
                var est = await _context.Establishments.FindAsync(dto.IdEstablishment);
                if (est == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                if (_userContext.UserRole != "SUPERADMIN" && dto.IdEstablishment != _userContext.EstablishmentId)
                    return new ResponseDto(403, message: "No tienes permisos para crear aquí.");

                var exists = await _context.CommonAreas
                    .AnyAsync(ca => ca.Name == dto.Name && ca.IdEstablishment == dto.IdEstablishment);
                if (exists)
                    return new ResponseDto(409, message: "Ya existe un área común con este nombre.");

                var ca = new CommonArea
                {
                    Name = dto.Name,
                    IdEstablishment = dto.IdEstablishment,
                    Mode = dto.Mode,
                    MaxCapacity = dto.Mode.HasFlag(CommonAreaMode.Usable) ? dto.MaxCapacity : null
                };

                _context.CommonAreas.Add(ca);
                await _context.SaveChangesAsync();

                await _auditLogService.LogManualAsync(
                    action: $"Creación de área común '{dto.Name}'",
                    email: _userContext.UserEmail, role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: "/CommonArea/create", httpMethod: "POST",
                    statusCode: 201
                );

                return new ResponseDto(201, ca, "Área común creada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Create CommonAreaService: " + ex.Message);
                return new ResponseDto(500, message: "Error al crear el área común.");
            }
        }

        public async Task<ResponseDto> UpdateAsync(int id, UpdateCommonAreaDto dto)
        {
            try
            {
                var ca = await _context.CommonAreas.FindAsync(id);
                if (ca == null)
                    return new ResponseDto(404, message: "Área común no encontrada.");

                if (_userContext.UserRole != "SUPERADMIN" && ca.IdEstablishment != _userContext.EstablishmentId)
                    return new ResponseDto(403, message: "No tienes permisos para actualizar esta área.");

                var changes = new List<string>();

                // Verificar si cambió el nombre
                if (dto.Name != ca.Name)
                {
                    changes.Add($"nombre: '{ca.Name}' → '{dto.Name}'");
                    ca.Name = dto.Name;
                }

                // Verificar si el modo cambia
                if (dto.Mode != ca.Mode)
                {
                    changes.Add($"modo cambiado: {ca.Mode} → {dto.Mode}");
                    ca.Mode = dto.Mode;
                }

                // Actualizar capacidad solo si el nuevo modo incluye Usable
                if (dto.Mode.HasFlag(CommonAreaMode.Usable))
                {
                    if (dto.MaxCapacity != ca.MaxCapacity)
                    {
                        changes.Add($"capacidad: {ca.MaxCapacity} → {dto.MaxCapacity}");
                        ca.MaxCapacity = dto.MaxCapacity;
                    }
                }
                else
                {
                    // Si ya no es usable, se borra la capacidad
                    if (ca.MaxCapacity != null)
                    {
                        changes.Add("capacidad eliminada (modo ya no es usable)");
                        ca.MaxCapacity = null;
                    }
                }

                // Verificar si cambió el estado
                if (dto.Status.HasValue && dto.Status != ca.Status)
                {
                    changes.Add($"estado: {ca.Status} → {dto.Status}");
                    ca.Status = dto.Status.Value;
                }

                await _context.SaveChangesAsync();

                await _auditLogService.LogManualAsync(
                    action: $"Actualización de área común '{ca.Name}': " + (changes.Any() ? string.Join(", ", changes) : "sin cambios"),
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: $"/CommonArea/update/{id}",
                    httpMethod: "PUT",
                    statusCode: 200
                );

                return new ResponseDto(200, ca, "Área común actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Update CommonAreaService: " + ex.Message);
                return new ResponseDto(500, message: "Error al actualizar el área común.");
            }
        }

        public async Task<ResponseDto> DeleteAsync(int id)
        {
            try
            {
                var ca = await _context.CommonAreas.FindAsync(id);
                if (ca == null)
                    return new ResponseDto(404, message: "Área común no encontrada.");

                if (_userContext.UserRole != "SUPERADMIN" && ca.IdEstablishment != _userContext.EstablishmentId)
                    return new ResponseDto(403, message: "No tienes permisos para eliminar esta área.");

                _context.CommonAreas.Remove(ca);
                await _context.SaveChangesAsync();

                await _auditLogService.LogManualAsync(
                    action: $"Eliminación de área común '{ca.Name}'",
                    email: _userContext.UserEmail, role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0, endpoint: $"/CommonArea/delete/{id}",
                    httpMethod: "DELETE", statusCode: 200
                );

                return new ResponseDto(200, message: "Área común eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Delete CommonAreaService: " + ex.Message);
                return new ResponseDto(500, message: "Error al eliminar el área común.");
            }
        }
    }
}
