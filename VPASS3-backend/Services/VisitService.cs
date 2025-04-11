using VPASS3_backend.Context;
using VPASS3_backend.DTOs.Visits;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace VPASS3_backend.Services
{
    public class VisitService : IVisitService
    {
        private readonly AppDbContext _context;

        public VisitService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> GetAllVisitsAsync()
        {
            try
            {
                var visits = await _context.Visits
                    .ToListAsync();

                return new ResponseDto(200, visits, "Visitas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllVisitsAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener las visitas.");
            }
        }

        public async Task<ResponseDto> GetVisitByIdAsync(int id)
        {
            try
            {
                var visit = await _context.Visits
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (visit == null)
                    return new ResponseDto(404, "Visita no encontrada.");

                return new ResponseDto(200, visit, "Visita obtenida correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetVisitByIdAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener la visita.");
            }
        }

        public async Task<ResponseDto> CreateVisitAsync(VisitDto dto)
        {
            try
            {
                var visit = new Visit
                {
                    EstablishmentId = dto.EstablishmentId,
                    VisitorId = dto.VisitorId,
                    ZoneId = dto.ZoneId,
                    EntryDate = dto.EntryDate
                };

                _context.Visits.Add(visit);
                await _context.SaveChangesAsync();

                return new ResponseDto(201, visit, "Visita creada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateVisitAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al crear la visita.");
            }
        }

        public async Task<ResponseDto> UpdateVisitAsync(int id, VisitDto dto)
        {
            try
            {
                var visit = await _context.Visits.FindAsync(id);
                if (visit == null)
                    return new ResponseDto(404, "Visita no encontrada.");

                visit.EstablishmentId = dto.EstablishmentId;
                visit.VisitorId = dto.VisitorId;
                visit.ZoneId = dto.ZoneId;
                visit.EntryDate = dto.EntryDate;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, visit, "Visita actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateVisitAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al actualizar la visita.");
            }
        }

        public async Task<ResponseDto> DeleteVisitAsync(int id)
        {
            try
            {
                var visit = await _context.Visits.FindAsync(id);
                if (visit == null)
                    return new ResponseDto(404, "Visita no encontrada.");

                _context.Visits.Remove(visit);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, "Visita eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteVisitAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al eliminar la visita.");
            }
        }
    }
}
