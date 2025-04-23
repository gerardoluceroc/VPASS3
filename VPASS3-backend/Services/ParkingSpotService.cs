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

        public ParkingSpotService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> GetAllParkingSpotsAsync()
        {
            try
            {
                var spots = await _context.ParkingSpots
                    .Include(p => p.Establishment)
                    .ToListAsync();

                return new ResponseDto(200, spots, "Estacionamientos obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllParkingSpotsAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener los estacionamientos.");
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
                    return new ResponseDto(404, "Estacionamiento no encontrado.");

                return new ResponseDto(200, spot, "Estacionamiento obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetParkingSpotByIdAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener el estacionamiento.");
            }
        }

        public async Task<ResponseDto> CreateParkingSpotAsync(ParkingSpotDto dto)
        {
            try
            {
                var establishment = await _context.Establishments.FindAsync(dto.IdEstablishment);
                if (establishment == null)
                    return new ResponseDto(404, "Establecimiento no encontrado.");

                var newSpot = new ParkingSpot
                {
                    Name = dto.Name,
                    IdEstablishment = dto.IdEstablishment
                };

                _context.ParkingSpots.Add(newSpot);
                await _context.SaveChangesAsync();

                return new ResponseDto(201, newSpot, "Estacionamiento creado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateParkingSpotAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al crear el estacionamiento.");
            }
        }

        public async Task<ResponseDto> UpdateParkingSpotAsync(int id, ParkingSpotDto dto)
        {
            try
            {
                var spot = await _context.ParkingSpots.FindAsync(id);
                if (spot == null)
                    return new ResponseDto(404, "Estacionamiento no encontrado.");

                var establishment = await _context.Establishments.FindAsync(dto.IdEstablishment);
                if (establishment == null)
                    return new ResponseDto(404, "Establecimiento no encontrado.");

                spot.Name = dto.Name;
                spot.IdEstablishment = dto.IdEstablishment;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, spot, "Estacionamiento actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateParkingSpotAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al actualizar el estacionamiento.");
            }
        }

        public async Task<ResponseDto> DeleteParkingSpotAsync(int id)
        {
            try
            {
                var spot = await _context.ParkingSpots.FindAsync(id);
                if (spot == null)
                    return new ResponseDto(404, "Estacionamiento no encontrado.");

                _context.ParkingSpots.Remove(spot);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, "Estacionamiento eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteParkingSpotAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al eliminar el estacionamiento.");
            }
        }
    }
}
