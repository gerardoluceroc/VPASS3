using VPASS3_backend.Context;
using VPASS3_backend.DTOs.Visitors;
using VPASS3_backend.DTOs;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Interfaces;

namespace VPASS3_backend.Services
{
    public class VisitorService : IVisitorService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public VisitorService(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }


        public async Task<ResponseDto> GetAllVisitorsAsync()
        {
            try
            {
                var visitors = await _context.Visitors
                    //.Include(v => v.Visits)
                    .ToListAsync();

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    visitors = visitors
                        .Where(v => _userContext.CanAccessVisitor(v))
                        .ToList();
                }

                return new ResponseDto(200, visitors, "Visitantes obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllVisitorsAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener los visitantes.");
            }
        }


        public async Task<ResponseDto> GetVisitorByIdAsync(int id)
        {
            try
            {
                var visitor = await _context.Visitors
                    .Include(v => v.Visits)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (visitor == null)
                    return new ResponseDto(404, message: "Visitante no encontrado.");

                if (!_userContext.CanAccessVisitor(visitor))
                    return new ResponseDto(403, message: "No tienes permiso para acceder a este visitante.");

                return new ResponseDto(200, visitor, "Visitante obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetVisitorByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener el visitante.");
            }
        }

        // Obtener visitante por rut o pasaporte
        public async Task<ResponseDto> GetVisitorByIdentificationNumberAsync(string identificationNumber)
        {
            try
            {
                var visitor = await _context.Visitors
                    .Include(v => v.Visits)
                    .FirstOrDefaultAsync(v => v.IdentificationNumber == identificationNumber);

                if (visitor == null)
                    return new ResponseDto(404, message: "Visitante no encontrado.");

                if (!_userContext.CanAccessVisitor(visitor))
                    return new ResponseDto(403, message: "No tienes permiso para acceder a este visitante.");

                return new ResponseDto(200, visitor, "Visitante obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetVisitorByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener el visitante.");
            }
        }


        public async Task<ResponseDto> CreateVisitorAsync(VisitorDto dto)
        {
            try
            {
                var exists = await _context.Visitors
                    .AnyAsync(v => v.IdentificationNumber == dto.IdentificationNumber);

                if (exists)
                    return new ResponseDto(409, message: "Ya existe un visitante con ese número de identificación.");

                // No hay visitas asociadas al momento de crear, por lo tanto, permitimos la creación libre
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
                return new ResponseDto(500, message: "Error en el servidor al crear el visitante.");
            }
        }


        public async Task<ResponseDto> UpdateVisitorAsync(int id, VisitorDto dto)
        {
            try
            {
                var visitor = await _context.Visitors
                    .Include(v => v.Visits)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (visitor == null)
                    return new ResponseDto(404, message: "Visitante no encontrado.");

                if (!_userContext.CanAccessVisitor(visitor))
                    return new ResponseDto(403, message: "No tienes permiso para modificar este visitante.");

                var duplicate = await _context.Visitors
                    .AnyAsync(v => v.IdentificationNumber == dto.IdentificationNumber && v.Id != id);

                if (duplicate)
                    return new ResponseDto(400, message: "Ya existe otro visitante con ese número de identificación.");

                visitor.Names = dto.Names;
                visitor.LastNames = dto.LastNames;
                visitor.IdentificationNumber = dto.IdentificationNumber;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, visitor, "Visitante actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateVisitorAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al actualizar el visitante.");
            }
        }

        public async Task<ResponseDto> DeleteVisitorAsync(int id)
        {
            try
            {
                var visitor = await _context.Visitors
                    .Include(v => v.Visits)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (visitor == null)
                    return new ResponseDto(404, message: "Visitante no encontrado.");

                if (!_userContext.CanAccessVisitor(visitor))
                    return new ResponseDto(403, message: "No tienes permiso para eliminar este visitante.");

                // Validar si el visitante tiene visitas asociadas
                if (visitor.Visits.Any())
                    return new ResponseDto(400, message: "No se puede eliminar el visitante porque tiene visitas asociadas.");

                _context.Visitors.Remove(visitor);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, message: "Visitante eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteVisitorAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar el visitante.");
            }
        }


    }
}