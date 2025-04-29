using VPASS3_backend.Context;
using VPASS3_backend.DTOs.VisitTypes;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace VPASS3_backend.Services
{
    public class VisitTypeService : IVisitTypeService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public VisitTypeService(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<ResponseDto> GetAllVisitTypesAsync()
        {
            try
            {
                var visitTypes = await _context.VisitTypes.ToListAsync();

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message:"No tienes un establecimiento asociado.");

                    visitTypes = visitTypes
                        .Where(vt => _userContext.CanAccessVisitType(vt))
                        .ToList();
                }

                return new ResponseDto(200, visitTypes, "Tipos de visita obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllVisitTypesAsync: " + ex.Message);
                return new ResponseDto(500, message:"Error en el servidor al obtener los tipos de visita.");
            }
        }

        public async Task<ResponseDto> GetVisitTypeByIdAsync(int id)
        {
            try
            {
                var visitType = await _context.VisitTypes.FirstOrDefaultAsync(vt => vt.Id == id);

                if (visitType == null)
                    return new ResponseDto(404, "Tipo de visita no encontrado.");

                if (!_userContext.CanAccessVisitType(visitType))
                    return new ResponseDto(403, message: "No tienes permiso para acceder a este tipo de visita.");

                return new ResponseDto(200, visitType, "Tipo de visita obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetVisitTypeByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener el tipo de visita.");
            }
        }

        public async Task<ResponseDto> CreateVisitTypeAsync(VisitTypeDto dto)
        {
            try
            {
                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != dto.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permiso para crear tipos de visita en este establecimiento.");
                }

                var duplicate = await _context.VisitTypes
                    .FirstOrDefaultAsync(vt =>
                        vt.IdEstablishment == dto.IdEstablishment &&
                        vt.Name.ToLower() == dto.Name.ToLower());

                if (duplicate != null)
                {
                    return new ResponseDto(409, message:$"Ya existe un tipo de visita con el nombre '{dto.Name}' para este establecimiento.");
                }

                var visitType = new VisitType
                {
                    Name = dto.Name,
                    IdEstablishment = dto.IdEstablishment
                };

                _context.VisitTypes.Add(visitType);
                await _context.SaveChangesAsync();

                return new ResponseDto(201, visitType, "Tipo de visita creado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateVisitTypeAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al crear el tipo de visita.");
            }
        }

        public async Task<ResponseDto> UpdateVisitTypeAsync(int id, VisitTypeDto dto)
        {
            try
            {
                var visitType = await _context.VisitTypes.FirstOrDefaultAsync(vt => vt.Id == id);

                if (visitType == null)
                    return new ResponseDto(404, message: "Tipo de visita no encontrado.");

                if (!_userContext.CanAccessVisitType(visitType))
                    return new ResponseDto(403, message: "No tienes permiso para editar este tipo de visita.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != dto.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No puedes reasignar el tipo de visita a otro establecimiento.");
                }

                var duplicate = await _context.VisitTypes
                    .FirstOrDefaultAsync(vt =>
                        vt.IdEstablishment == dto.IdEstablishment &&
                        vt.Name.ToLower() == dto.Name.ToLower() &&
                        vt.Id != id);

                if (duplicate != null)
                {
                    return new ResponseDto(409, message: $"Ya existe un tipo de visita con el nombre '{dto.Name}' para este establecimiento.");
                }

                visitType.Name = dto.Name;
                visitType.IdEstablishment = dto.IdEstablishment;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, visitType, "Tipo de visita actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateVisitTypeAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al actualizar el tipo de visita.");
            }
        }

        public async Task<ResponseDto> DeleteVisitTypeAsync(int id)
        {
            try
            {
                var visitType = await _context.VisitTypes.FirstOrDefaultAsync(vt => vt.Id == id);

                if (visitType == null)
                    return new ResponseDto(404, message: "Tipo de visita no encontrado.");

                if (!_userContext.CanAccessVisitType(visitType))
                    return new ResponseDto(403, message: "No tienes permiso para eliminar este tipo de visita.");

                _context.VisitTypes.Remove(visitType);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, message: "Tipo de visita eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteVisitTypeAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar el tipo de visita.");
            }
        }

    }
}
