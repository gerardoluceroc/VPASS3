using VPASS3_backend.Context;
using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.DTOs;
using VPASS3_backend.Enums;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models.CommonAreas.ReservableCommonArea;
using VPASS3_backend.Models.CommonAreas.UsableCommonArea;
using VPASS3_backend.Models.CommonAreas;
using Microsoft.EntityFrameworkCore;

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

        public async Task<ResponseDto> GetAllCommonAreasAsync()
        {
            try
            {
                var areas = await _context.CommonAreas
                    .Include(ca => ca.Establishment)
                    .ToListAsync();

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    areas = areas
                        .Where(ca => ca.IdEstablishment == _userContext.EstablishmentId)
                        .ToList();
                }

                return new ResponseDto(200, areas, "Áreas comunes obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllCommonAreasAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error al obtener las áreas comunes.");
            }
        }

        public async Task<ResponseDto> GetCommonAreaByIdAsync(int id)
        {
            try
            {
                var area = await _context.CommonAreas
                    .Include(ca => ca.Establishment)
                    .FirstOrDefaultAsync(ca => ca.Id == id);

                if (area == null)
                    return new ResponseDto(404, message: "Área común no encontrada.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != area.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permisos para ver esta área común.");
                }

                return new ResponseDto(200, area, "Área común obtenida correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetCommonAreaByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error al obtener el área común.");
            }
        }

        public async Task<ResponseDto> CreateCommonAreaAsync(CreateCommonAreaDto dto)
        {
            try
            {
                var establishment = await _context.Establishments.FindAsync(dto.IdEstablishment);
                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != dto.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permisos para registrar esta área.");
                }

                var exists = await _context.CommonAreas.AnyAsync(ca =>
                    ca.Name == dto.Name && ca.IdEstablishment == dto.IdEstablishment);

                if (exists)
                    return new ResponseDto(409, message: "Ya existe un área común con este nombre.");

                CommonArea commonArea;

                if (dto.Type == CommonAreaType.Reservable)
                {
                    commonArea = new ReservableCommonArea
                    {
                        Name = dto.Name,
                        IdEstablishment = dto.IdEstablishment,
                        Type = dto.Type,
                    };
                }
                else if (dto.Type == CommonAreaType.Usable)
                {
                    commonArea = new UsableCommonArea
                    {
                        Name = dto.Name,
                        IdEstablishment = dto.IdEstablishment,
                        Type = dto.Type,
                        MaxCapacity = dto.MaxCapacity ?? 0
                    };
                }
                else
                {
                    return new ResponseDto(400, message: "Tipo de área común inválido.");
                }

                _context.CommonAreas.Add(commonArea);
                await _context.SaveChangesAsync();

                await _auditLogService.LogManualAsync(
                    action: $"Se creó el área común '{dto.Name}'",
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: "/CommonArea/create",
                    httpMethod: "POST",
                    statusCode: 201
                );

                return new ResponseDto(201, commonArea, "Área común creada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateCommonAreaAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error al crear el área común.");
            }
        }

        public async Task<ResponseDto> UpdateCommonAreaAsync(int id, CreateCommonAreaDto dto)
        {
            try
            {
                var commonArea = await _context.CommonAreas.FindAsync(id);
                if (commonArea == null)
                    return new ResponseDto(404, message: "Área común no encontrada.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != commonArea.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permisos para modificar esta área.");
                }

                commonArea.Name = dto.Name;

                if (commonArea is UsableCommonArea usable && dto.Type == CommonAreaType.Usable)
                {
                    usable.MaxCapacity = dto.MaxCapacity ?? usable.MaxCapacity;
                }

                await _context.SaveChangesAsync();

                await _auditLogService.LogManualAsync(
                    action: $"Se actualizó el área común '{commonArea.Name}'",
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: $"/CommonArea/update/{id}",
                    httpMethod: "PUT",
                    statusCode: 200
                );

                return new ResponseDto(200, commonArea, "Área común actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateCommonAreaAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error al actualizar el área común.");
            }
        }

        public async Task<ResponseDto> DeleteCommonAreaAsync(int id)
        {
            try
            {
                var commonArea = await _context.CommonAreas.FindAsync(id);
                if (commonArea == null)
                    return new ResponseDto(404, message: "Área común no encontrada.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != commonArea.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permisos para eliminar esta área.");
                }

                _context.CommonAreas.Remove(commonArea);
                await _context.SaveChangesAsync();

                await _auditLogService.LogManualAsync(
                    action: $"Se eliminó el área común '{commonArea.Name}'",
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: $"/CommonArea/delete/{id}",
                    httpMethod: "DELETE",
                    statusCode: 200
                );

                return new ResponseDto(200, message: "Área común eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteCommonAreaAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error al eliminar el área común.");
            }
        }
    }
}