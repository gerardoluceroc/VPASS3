using VPASS3_backend.Context;
using VPASS3_backend.DTOs.Blacklist;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace VPASS3_backend.Services
{
    public class BlacklistService : IBlacklistService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;
        private readonly IAuditLogService _auditLogService;

        public BlacklistService(AppDbContext context, IUserContextService userContext, IAuditLogService auditLogService)
        {
            _context = context;
            _userContext = userContext;
            _auditLogService = auditLogService;
        }

        public async Task<ResponseDto> CreateAsync(BlacklistDto dto)
        {
            try
            {
                var person = await _context.Persons.FindAsync(dto.IdPerson);
                if (person == null)
                    return new ResponseDto(404, message: "Persona no encontrada.");

                var establishment = await _context.Establishments.FindAsync(dto.IdEstablishment);
                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                if (!_userContext.CanAccessOwnEstablishment(dto.IdEstablishment))
                    return new ResponseDto(403, message: "No tienes permiso para agregar a este establecimiento.");

                var exists = await _context.Blacklists.AnyAsync(b =>
                    b.IdPerson == dto.IdPerson && b.IdEstablishment == dto.IdEstablishment);

                if (exists)
                    return new ResponseDto(409, message: "La persona ya se encuentra en la lista negra de este establecimiento.");
                 
                var entry = new Blacklist
                {
                    IdPerson = dto.IdPerson,
                    IdEstablishment = dto.IdEstablishment,
                    Reason = dto.Reason
                };

                _context.Blacklists.Add(entry);
                await _context.SaveChangesAsync();

                // Registrar en AuditLog
                var auditLogMessage = $"El visitante {person.Names} {person.LastNames} " +
                                  $"(Rut/Pasaporte: {person.IdentificationNumber}) fue añadido a la lista negra " +
                                  $"del establecimiento {establishment.Name}, ID: {dto.IdEstablishment}.";

                await _auditLogService.LogManualAsync(
                    action: auditLogMessage,
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: "/blacklist/create",
                    httpMethod: "POST",
                    statusCode: 201
                );

                return new ResponseDto(201, entry, "Persona añadida a la lista negra.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error del servidor.");
            }
        }

        public async Task<ResponseDto> DeleteByPersonAsync(DeleteBlacklistByPersonIdDto dto)
        {
            try
            {
                var person = await _context.Persons.FindAsync(dto.IdPerson);
                if (person == null)
                    return new ResponseDto(404, message: "Persona no encontrada.");

                var establishment = await _context.Establishments.FindAsync(dto.IdEstablishment);
                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                if (!_userContext.CanAccessOwnEstablishment(dto.IdEstablishment))
                    return new ResponseDto(403, message: "No tienes permisos para eliminar en este establecimiento.");

                var blacklistEntry = await _context.Blacklists
                    .FirstOrDefaultAsync(b =>
                        b.IdPerson == dto.IdPerson &&
                        b.IdEstablishment == dto.IdEstablishment);

                if (blacklistEntry == null)
                    return new ResponseDto(404, message: "Campo de lista negra no encontrada.");

                _context.Blacklists.Remove(blacklistEntry);
                await _context.SaveChangesAsync();

                // Registrar en AuditLog
                var message = $"La persona {person.Names} {person.LastNames} " +
                              $"(Rut/Pasaporte: {person.IdentificationNumber}) fue eliminada de la lista negra " +
                              $"del establecimiento {establishment.Name}, ID: {dto.IdEstablishment}.";

                await _auditLogService.LogManualAsync(
                    action: message,
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: "/blacklist/deleteByPersonId",
                    httpMethod: "POST",
                    statusCode: 200
                );

                return new ResponseDto(200, message: "Persona eliminada de la lista negra.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteByVisitorAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error del servidor.");
            }
        }

        public async Task<ResponseDto> UpdateAsync(int id, BlacklistDto dto)
        {
            try
            {
                var blacklist = await _context.Blacklists.FindAsync(id);
                if (blacklist == null)
                    return new ResponseDto(404, message: "Entrada no encontrada.");

                // Verificación de acceso (al establecimiento actual y al nuevo si cambia)
                if (_userContext.UserRole != "SUPERADMIN" &&
                    (!_userContext.CanAccessOwnEstablishment(blacklist.IdEstablishment) ||
                     !_userContext.CanAccessOwnEstablishment(dto.IdEstablishment)))
                {
                    return new ResponseDto(403, message: "No tienes permisos para modificar esta entrada.");
                }

                var person = await _context.Persons.FindAsync(dto.IdPerson);
                if (person == null)
                    return new ResponseDto(404, message: "Persona no encontrada.");

                var establishment = await _context.Establishments.FindAsync(dto.IdEstablishment);
                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                // Validar si se intenta duplicar una entrada existente
                var alreadyExists = await _context.Blacklists.AnyAsync(b =>
                    b.Id != id &&
                    b.IdPerson == dto.IdPerson &&
                    b.IdEstablishment == dto.IdEstablishment);

                if (alreadyExists)
                    return new ResponseDto(409, message: "Ya existe una entrada similar en la lista negra.");

                // Actualizar valores
                blacklist.IdPerson = dto.IdPerson;
                blacklist.IdEstablishment = dto.IdEstablishment;
                blacklist.Reason = dto.Reason;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, blacklist, "Entrada actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error del servidor.");
            }
        }



        public async Task<ResponseDto> GetAllAsync()
        {
            try
            {
                var query = _context.Blacklists
                    .Include(b => b.Person)
                    .Include(b => b.Establishment)
                    .AsQueryable();

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "Establecimiento no asociado.");

                    query = query.Where(b => b.IdEstablishment == _userContext.EstablishmentId.Value);
                }

                var list = await query.ToListAsync();
                return new ResponseDto(200, list, "Lista negra obtenida.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error del servidor.");
            }
        }

        public async Task<ResponseDto> GetByIdAsync(int id)
        {
            try
            {
                var entry = await _context.Blacklists
                    .Include(b => b.Person)
                    .Include(b => b.Establishment)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (entry == null)
                    return new ResponseDto(404, message: "Entrada no encontrada.");

                if (!_userContext.CanAccessOwnEstablishment(entry.IdEstablishment))
                    return new ResponseDto(403, message: "No tienes permiso para ver esta entrada.");

                return new ResponseDto(200, entry, "Entrada obtenida.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error del servidor.");
            }
        }

        public async Task<ResponseDto> DeleteAsync(int id)
        {
            try
            {
                var entry = await _context.Blacklists.FindAsync(id);
                if (entry == null)
                    return new ResponseDto(404, message: "Entrada no encontrada.");

                if (!_userContext.CanAccessOwnEstablishment(entry.IdEstablishment))
                    return new ResponseDto(403, message: "No tienes permiso para eliminar esta entrada.");

                _context.Blacklists.Remove(entry);
                await _context.SaveChangesAsync();

                var person = await _context.Persons.FindAsync(entry.IdPerson);

                // Registrar en AuditLog
                var message = $"La persona {person.Names} {person.LastNames} " +
                              $"(Rut/Pasaporte: {person.IdentificationNumber}) fue eliminada de la lista negra " +
                              $"del establecimiento {entry.Establishment.Name} con ID: {entry.IdEstablishment}";

                await _auditLogService.LogManualAsync(
                    action: message,
                    email: _userContext.UserEmail,
                    role: _userContext.UserRole,
                    userId: _userContext.UserId ?? 0,
                    endpoint: "/blacklist/delete/{id}",
                    httpMethod: "DELETE",
                    statusCode: 200
                );

                return new ResponseDto(200, message: "Entrada eliminada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error del servidor.");
            }
        }
    }
}