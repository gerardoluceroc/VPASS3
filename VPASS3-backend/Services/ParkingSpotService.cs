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

        public ParkingSpotService(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
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
                return new ResponseDto(500, message:"Error en el servidor al obtener los estacionamientos.");
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
                    IdEstablishment = dto.IdEstablishment
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


        public async Task<ResponseDto> UpdateParkingSpotAsync(int id, ParkingSpotDto dto)
        {
            try
            {
                var spot = await _context.ParkingSpots
                    .Include(p => p.Establishment)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (spot == null)
                    return new ResponseDto(404, message: "Estacionamiento no encontrado.");

                if (!_userContext.CanAccessParkingSpot(spot))
                    return new ResponseDto(403, message: "No tienes permiso para editar este estacionamiento.");

                var establishment = await _context.Establishments.FindAsync(dto.IdEstablishment);
                if (establishment == null)
                    return new ResponseDto(404, message: "Establecimiento no encontrado.");

                // Validar también si el cambio de establecimiento es válido
                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != dto.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No puedes mover este estacionamiento a otro establecimiento.");
                }

                spot.Name = dto.Name;
                spot.IdEstablishment = dto.IdEstablishment;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, spot, "Estacionamiento actualizado correctamente.");
            }
            catch (Exception ex)
            {
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
