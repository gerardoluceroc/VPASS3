using VPASS3_backend.Context;
using VPASS3_backend.DTOs.Visitors;
using VPASS3_backend.DTOs;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace VPASS3_backend.Services
{
    public class VisitorService : IVisitorService
    {
        private readonly AppDbContext _context;

        public VisitorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> GetAllVisitorsAsync()
        {
            try
            {
                var visitors = await _context.Visitors.ToListAsync();
                return new ResponseDto(200, visitors, "Visitantes obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllVisitorsAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener los visitantes.");
            }
        }

        public async Task<ResponseDto> GetVisitorByIdAsync(int id)
        {
            try
            {
                var visitor = await _context.Visitors.FindAsync(id);
                if (visitor == null)
                    return new ResponseDto(404, "Visitante no encontrado.");

                return new ResponseDto(200, visitor, "Visitante obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetVisitorByIdAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener el visitante.");
            }
        }

        public async Task<ResponseDto> CreateVisitorAsync(VisitorDto dto)
        {
            try
            {
                var exists = await _context.Visitors
                    .AnyAsync(v => v.IdentificationNumber == dto.IdentificationNumber);

                if (exists)
                    return new ResponseDto(400, "Ya existe un visitante con ese número de identificación.");

                var visitor = new Visitor
                {
                    Names = dto.Names,
                    LastNames = dto.LastNames,
                    IdentificationNumber = dto.IdentificationNumber
                };

                _context.Visitors.Add(visitor);
                await _context.SaveChangesAsync();

                return new ResponseDto(201, visitor, "Visitante creado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateVisitorAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al crear el visitante.");
            }
        }

        public async Task<ResponseDto> UpdateVisitorAsync(int id, VisitorDto dto)
        {
            try
            {
                var existingVisitor = await _context.Visitors.FindAsync(id);
                if (existingVisitor == null)
                    return new ResponseDto(404, "Visitante no encontrado.");

                var duplicate = await _context.Visitors
                    .AnyAsync(v => v.IdentificationNumber == dto.IdentificationNumber && v.Id != id);

                if (duplicate)
                    return new ResponseDto(400, "Ya existe otro visitante con ese número de identificación.");

                existingVisitor.Names = dto.Names;
                existingVisitor.LastNames = dto.LastNames;
                existingVisitor.IdentificationNumber = dto.IdentificationNumber;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, existingVisitor, "Visitante actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateVisitorAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al actualizar el visitante.");
            }
        }

        public async Task<ResponseDto> DeleteVisitorAsync(int id)
        {
            try
            {
                var visitor = await _context.Visitors.FindAsync(id);
                if (visitor == null)
                    return new ResponseDto(404, "Visitante no encontrado.");

                _context.Visitors.Remove(visitor);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, "Visitante eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteVisitorAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al eliminar el visitante.");
            }
        }
    }
}