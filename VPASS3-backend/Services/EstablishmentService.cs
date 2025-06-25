using VPASS3_backend.Context;
using VPASS3_backend.DTOs.Establishments;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;


namespace VPASS3_backend.Services
{
    public class EstablishmentService : IEstablishmentService
    {
        private readonly AppDbContext _context;

        private readonly IUserContextService _userContext;

        private readonly IAuditLogService _auditLogService;

        public EstablishmentService(AppDbContext context, IUserContextService userContext, IAuditLogService auditLogService)
        {
            _context = context;
            _userContext = userContext;
            _auditLogService = auditLogService;
        }

        public async Task<ResponseDto> GetAllEstablishmentsAsync()
        {
            try
            {
                var establishments = await _context.Establishments
                    .Include(e => e.Users)
                    .Include(e => e.Zones)
                        .ThenInclude(z => z.Apartments)  // Esto agrega los departamentos dentro de cada zona
                    .Include(e => e.ParkingSpots)
                    .Include(e => e.Blacklists)
                    .Include(e => e.CommonAreas)
                    .ToListAsync();

                return new ResponseDto(200, establishments, "Establecimientos obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllEstablishmentsAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener los establecimientos.");
            }
        }

        public async Task<ResponseDto> GetEstablishmentByIdAsync(int id)
        {
            try
            {
                // Se verifica que el usuario sea Super Admin o que esté consultando a recursos relacionados con su usuario
                if (!_userContext.CanAccessOwnEstablishment(id))
                {
                    return new ResponseDto(403, message: "No cuenta con los permisos para ver la información de otros usuarios");
                }

                var establishment = await _context.Establishments
                    .Include(e => e.Users)
                    .Include(e => e.Zones)
                    .Include(e => e.ParkingSpots)
                    .Include(e => e.Blacklists)
                    .Include(e => e.CommonAreas)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                return new ResponseDto(200, establishment, "Establecimiento obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetEstablishmentByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener el establecimiento.");
            }
        }

        public async Task<ResponseDto> CreateEstablishmentAsync(CreateEstablishmentDto dto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (user == null)
                    return new ResponseDto(404, message: $"No se encontró ningún usuario con el email ingresado.");

                if (user.EstablishmentId != null)
                    return new ResponseDto(409, message: "El usuario ya está asociado a un establecimiento y no puede ser asignado a otro.");

                // Crear establecimiento
                var establishment = new Establishment
                {
                    Name = dto.Name
                };

                _context.Establishments.Add(establishment);
                await _context.SaveChangesAsync(); // Aquí se genera el Id

                // Asociar el usuario al establecimiento creado
                user.EstablishmentId = establishment.Id;
                await _context.SaveChangesAsync();

                var message = $"El establecimiento {establishment.Name} con ID {establishment.Id} fue creado y asignado al usuario {user.Email}.";

                await _auditLogService.LogManualAsync(
                    action: message,
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: "/establishment/create",
                    httpMethod: "POST",
                    statusCode: 201
                );

                return new ResponseDto(201, establishment, "Establecimiento creado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateEstablishmentAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al crear el establecimiento.");
            }
        }

        public async Task<ResponseDto> UpdateEstablishmentAsync(int id, CreateEstablishmentDto dto)
        {
            try
            {
                // Se verifica que el usuario sea Super Admin o que esté consultando a recursos relacionados con su usuario
                if (!_userContext.CanAccessOwnEstablishment(id))
                {
                    return new ResponseDto(403, message: "No cuenta con los permisos para administrar la información de otros usuarios");
                }

                var establishment = await _context.Establishments
                    .Include(e => e.Users)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                establishment.Name = dto.Name;

                if (!string.IsNullOrEmpty(dto.Email))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

                    if (user == null)
                        return new ResponseDto(404, message: $"No se encontró ningún usuario con el email ingresado");

                    // Verificar si el usuario ya está asociado a otro establecimiento
                    if (user.EstablishmentId != null && user.EstablishmentId != establishment.Id)
                    {
                        return new ResponseDto(409, message: "El usuario ya está asociado a otro establecimiento.");
                    }

                    // Asociar el usuario actual a este establecimiento
                    user.EstablishmentId = establishment.Id;

                    // Nota: Ya no hay necesidad de modificar navegación inversa ya que se eliminó la relación uno a uno
                }

                await _context.SaveChangesAsync();
                return new ResponseDto(200, establishment, "Establecimiento actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateEstablishmentAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al actualizar el establecimiento.");
            }
        }

        public async Task<ResponseDto> DeleteEstablishmentAsync(int id)
        {
            try
            {
                // Se verifica que el usuario sea Super Admin o que esté consultando a recursos relacionados con su usuario
                if (!_userContext.CanAccessOwnEstablishment(id))
                {
                    return new ResponseDto(403, message: "No cuenta con los permisos para administrar la información de otros usuarios");
                }

                var establishment = await _context.Establishments.FindAsync(id);

                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                _context.Establishments.Remove(establishment);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, message: "Establecimiento eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteEstablishmentAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar el establecimiento.");
            }
        }
    }
}