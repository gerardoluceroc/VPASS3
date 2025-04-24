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

        public VisitTypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> GetAllVisitTypesAsync()
        {
            try
            {
                var visitTypes = await _context.VisitTypes.ToListAsync();
                return new ResponseDto(200, visitTypes, "Tipos de visita obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllVisitTypesAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener los tipos de visita.");
            }
        }

        public async Task<ResponseDto> GetVisitTypeByIdAsync(int id)
        {
            try
            {
                var visitType = await _context.VisitTypes.FindAsync(id);

                if (visitType == null)
                    return new ResponseDto(404, "Tipo de visita no encontrado.");

                return new ResponseDto(200, visitType, "Tipo de visita obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetVisitTypeByIdAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener el tipo de visita.");
            }
        }

        public async Task<ResponseDto> CreateVisitTypeAsync(VisitTypeDto dto)
        {
            try
            {
                // 1. Verificar que el establecimiento exista
                var establishment = await _context.Establishments
                    .FirstOrDefaultAsync(e => e.Id == dto.IdEstablishment);

                if (establishment == null)
                {
                    return new ResponseDto(404, "El establecimiento especificado no existe.");
                }

                // 2. Verificar si ya existe un VisitType con el mismo nombre para ese establecimiento
                var existingVisitType = await _context.VisitTypes
                    .FirstOrDefaultAsync(vt => vt.IdEstablishment == dto.IdEstablishment && vt.Name.ToLower() == dto.Name.ToLower());

                if (existingVisitType != null)
                {
                    return new ResponseDto(409, $"Ya existe un tipo de visita con el nombre '{dto.Name}' para este establecimiento.");
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
                return new ResponseDto(500, "Error en el servidor al crear el tipo de visita.");
            }
        }

        public async Task<ResponseDto> UpdateVisitTypeAsync(int id, VisitTypeDto dto)
        {
            try
            {
                // 1. Verificar que el establecimiento exista
                var establishment = await _context.Establishments
                    .FirstOrDefaultAsync(e => e.Id == dto.IdEstablishment);

                if (establishment == null)
                {
                    return new ResponseDto(404, "El establecimiento especificado no existe.");
                }

                var visitType = await _context.VisitTypes.FindAsync(id);

                if (visitType == null)
                {
                    return new ResponseDto(404, "Tipo de visita no encontrado.");
                }

                // 2. Verificar si ya existe un VisitType con ese nombre para el mismo establecimiento (excluyendo el actual)
                var duplicateVisitType = await _context.VisitTypes
                    .FirstOrDefaultAsync(vt =>
                        vt.Id != id &&
                        vt.IdEstablishment == dto.IdEstablishment &&
                        vt.Name.ToLower() == dto.Name.ToLower());

                if (duplicateVisitType != null)
                {
                    return new ResponseDto(409, $"Ya existe un tipo de visita con el nombre '{dto.Name}' para este establecimiento.");
                }

                // 3. Actualizar campos
                visitType.Name = dto.Name;
                visitType.IdEstablishment = dto.IdEstablishment;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, visitType, "Tipo de visita actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateVisitTypeAsync: " + ex.Message);    
                return new ResponseDto(500, "Error en el servidor al actualizar el tipo de visita.");
            }
        }

        public async Task<ResponseDto> DeleteVisitTypeAsync(int id)
        {
            try
            {
                var visitType = await _context.VisitTypes.FindAsync(id);

                if (visitType == null)
                    return new ResponseDto(404, "Tipo de visita no encontrado.");

                _context.VisitTypes.Remove(visitType);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, "Tipo de visita eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteVisitTypeAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al eliminar el tipo de visita.");
            }
        }
    }
}
