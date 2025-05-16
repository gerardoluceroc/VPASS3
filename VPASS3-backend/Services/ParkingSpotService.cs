using VPASS3_backend.Context;
using VPASS3_backend.DTOs.ParkingSpots;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace VPASS3_backend.Services
{
    public class ParkingSpotService : IParkingSpotService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;
        private readonly IAuditLogService _auditLogService;

        public ParkingSpotService(AppDbContext context, IUserContextService userContext, IAuditLogService auditLogService)
        {
            _context = context;
            _userContext = userContext;
            _auditLogService = auditLogService; 
        }

        public async Task<ResponseDto> GetAllParkingSpotsAsync()
        {
            try
            {
                var spots = await _context.ParkingSpots
                    .Include(p => p.Establishment)
                    .ToListAsync();

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message:"No tienes un establecimiento asociado.");

                    spots = spots
                        .Where(s => _userContext.CanAccessParkingSpot(s))
                        .ToList();
                }

                return new ResponseDto(200, spots, "Estacionamientos obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllParkingSpotsAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener los estacionamientos.");
            }
        }


        public async Task<ResponseDto> GetParkingSpotByIdAsync(int id)
        {
            try
            {
                var spot = await _context.ParkingSpots
                    .Include(p => p.Establishment)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (spot == null)
                    return new ResponseDto(404, message: "Estacionamiento no encontrado.");

                if (!_userContext.CanAccessParkingSpot(spot))
                    return new ResponseDto(403, message: "No tienes permiso para acceder a este estacionamiento.");

                return new ResponseDto(200, spot, "Estacionamiento obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetParkingSpotByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener el estacionamiento.");
            }
        }


        public async Task<ResponseDto> CreateParkingSpotAsync(ParkingSpotDto dto)
        {
            try
            {
                var establishment = await _context.Establishments.FindAsync(dto.IdEstablishment);
                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                // Validar acceso
                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != dto.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permiso para crear un estacionamiento en este establecimiento.");
                }

                var newSpot = new ParkingSpot
                {
                    Name = dto.Name,
                    IdEstablishment = dto.IdEstablishment,
                    IsAvailable = true // Valor por defecto
                };

                _context.ParkingSpots.Add(newSpot);
                await _context.SaveChangesAsync();

                return new ResponseDto(201, newSpot, "Estacionamiento creado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateParkingSpotAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al crear el estacionamiento.");
            }
        }

        public async Task<ResponseDto> UpdateParkingSpotAsync(int id, UpdateParkingSpotDto dto)
        {
            try
            {
                // Buscar el estacionamiento por ID, incluyendo la relación con el establecimiento
                var spot = await _context.ParkingSpots
                    .Include(p => p.Establishment)
                    .FirstOrDefaultAsync(p => p.Id == id);

                // Si no se encuentra, retornar 404
                if (spot == null)
                    return new ResponseDto(404, message: "Estacionamiento no encontrado.");

                // Validar que el usuario tenga permisos para editar este estacionamiento
                if (!_userContext.CanAccessParkingSpot(spot))
                    return new ResponseDto(403, message: "No tienes permiso para editar este estacionamiento.");

                // Verificar que el nuevo establecimiento (si se cambia) exista
                var establishment = await _context.Establishments.FindAsync(dto.IdEstablishment);
                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                // Validar si el usuario tiene permisos para mover el estacionamiento a otro establecimiento
                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != dto.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No puedes mover este estacionamiento a otro establecimiento.");
                }

                // Validar que el nuevo nombre no esté repetido en el mismo establecimiento (si se proporciona uno nuevo)
                if (!string.IsNullOrEmpty(dto.Name) && dto.Name != spot.Name)
                {
                    bool nameExists = await _context.ParkingSpots
                        .AnyAsync(p => p.Name == dto.Name &&
                                       p.IdEstablishment == dto.IdEstablishment &&
                                       p.Id != id); // Excluir el mismo registro que se está actualizando

                    if (nameExists)
                        return new ResponseDto(409, message: "Ya existe un estacionamiento con ese nombre en este establecimiento.");
                }

                // Lista para acumular los cambios realizados y luego usarlos en el mensaje de auditoría
                var cambios = new List<string>();

                // Validar y aplicar el cambio de nombre, si corresponde
                if (!string.IsNullOrEmpty(dto.Name) && dto.Name != spot.Name)
                {
                    cambios.Add($"nombre cambiado de '{spot.Name}' a '{dto.Name}'");
                    spot.Name = dto.Name;
                }

                // Validar y aplicar el cambio de estado de disponibilidad, si corresponde
                if (dto.IsAvailable.HasValue && dto.IsAvailable != spot.IsAvailable)
                {
                    string antes = spot.IsAvailable == true ? "disponible" : "ocupado";
                    string despues = dto.IsAvailable == true ? "disponible" : "ocupado";
                    cambios.Add($"estado cambiado de {antes} a {despues}");
                    spot.IsAvailable = dto.IsAvailable;
                }

                // Validar y aplicar el cambio de establecimiento, si corresponde
                if (dto.IdEstablishment != spot.IdEstablishment)
                {
                    cambios.Add($"establecimiento cambiado de ID {spot.IdEstablishment} a ID {dto.IdEstablishment}");
                    spot.IdEstablishment = dto.IdEstablishment;
                }

                // Guardar los cambios en base de datos
                await _context.SaveChangesAsync();

                // Construir el mensaje de auditoría con los cambios registrados
                string auditMessage = cambios.Any()
                    //? "Estacionamiento actualizado: " + string.Join(", ", cambios)
                    ? $"{spot.Name} actualizado: " + string.Join(", ", cambios)
                    : "Actualización sin cambios significativos.";

                // Registrar la acción en el log de auditoría
                await _auditLogService.LogManualAsync(
                    action: auditMessage,
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0, // Si no hay ID de usuario, se usa 0 como valor por defecto
                    endpoint: $"/parking/update/{id}",
                    httpMethod: "PUT",
                    statusCode: 200
                );

                // Retornar la respuesta con los datos actualizados y mensaje de éxito
                return new ResponseDto(200, spot, "Estacionamiento actualizado correctamente.");
            }
            catch (Exception ex)
            {
                // Loguear error en consola y devolver respuesta de error 500
                Console.WriteLine("Error en UpdateParkingSpotAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al actualizar el estacionamiento.");
            }
        }


        public async Task<ResponseDto> DeleteParkingSpotAsync(int id)
        {
            try
            {
                var spot = await _context.ParkingSpots
                    .Include(p => p.Establishment)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (spot == null)
                    return new ResponseDto(404, message: "Estacionamiento no encontrado.");

                if (!_userContext.CanAccessParkingSpot(spot))
                    return new ResponseDto(403, message: "No tienes permiso para eliminar este estacionamiento.");

                _context.ParkingSpots.Remove(spot);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, message: "Estacionamiento eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteParkingSpotAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar el estacionamiento.");
            }
        }

    }
}
