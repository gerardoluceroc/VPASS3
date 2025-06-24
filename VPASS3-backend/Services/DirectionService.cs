using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Context;
using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.Directions;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;

namespace VPASS3_backend.Services
{
    public class DirectionService : IDirectionService
    {
        private readonly AppDbContext _context;

        public DirectionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> GetAllDirectionsAsync()
        {
            try
            {
                var directions = await _context.Directions.ToListAsync();
                return new ResponseDto(200, directions, "Sentidos obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllDirectionsAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener los sentidos.");
            }
        }

        public async Task<ResponseDto> GetDirectionByIdAsync(int id)
        {
            try
            {
                var direction = await _context.Directions.FirstOrDefaultAsync(d => d.Id == id);

                if (direction == null)
                    return new ResponseDto(404, message: "Sentido no encontrado.");

                return new ResponseDto(200, direction, "Sentido obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetDirectionByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener el sentido.");
            }
        }

        public async Task<ResponseDto> CreateDirectionAsync(DirectionDto dto)
        {
            try
            {
                var direction = new Direction
                {
                    VisitDirection = dto.VisitDirection!
                };

                _context.Directions.Add(direction);
                await _context.SaveChangesAsync();

                return new ResponseDto(201, direction, "Sentido creado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateDirectionAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al crear el sentido.");
            }
        }

        public async Task<ResponseDto> UpdateDirectionAsync(int id, DirectionDto dto)
        {
            try
            {
                var direction = await _context.Directions.FirstOrDefaultAsync(d => d.Id == id);

                if (direction == null)
                    return new ResponseDto(404, message: "Sentido no encontrado.");

                direction.VisitDirection = dto.VisitDirection!;
                await _context.SaveChangesAsync();

                return new ResponseDto(200, direction, "Sentido actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateDirectionAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al actualizar el sentido.");
            }
        }

        public async Task<ResponseDto> DeleteDirectionAsync(int id)
        {
            try
            {
                var direction = await _context.Directions.FindAsync(id);

                if (direction == null)
                    return new ResponseDto(404, message: "Sentido no encontrado.");

                _context.Directions.Remove(direction);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, message: "Sentido eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteDirectionAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar el sentido.");
            }
        }
    }
}