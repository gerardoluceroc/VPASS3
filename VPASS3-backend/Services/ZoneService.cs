using VPASS3_backend.Context;
using VPASS3_backend.DTOs.Zones;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace VPASS3_backend.Services
{
    public class ZoneService : IZoneService
    {
        private readonly AppDbContext _context;

        public ZoneService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> GetAllZonesAsync()
        {
            try
            {
                var zones = await _context.Zones
                    .Include(z => z.Establishment)
                    .Include(z => z.ZoneSections)
                    .ToListAsync();

                return new ResponseDto(200, zones, "Zonas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllZonesAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener las zonas.");
            }
        }

        public async Task<ResponseDto> GetZoneByIdAsync(int id)
        {
            try
            {
                var zone = await _context.Zones
                .Include(z => z.Establishment)
                .Include(z => z.ZoneSections)  
                .FirstOrDefaultAsync(z => z.Id == id);

                if (zone == null)
                    return new ResponseDto(404, "Zona no encontrada.");

                return new ResponseDto(200, zone, "Zona obtenida correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetZoneByIdAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener la zona.");
            }
        }

        public async Task<ResponseDto> CreateZoneAsync(CreateZoneDto dto)
        {
            try
            {
                // Verificar si el Establishment existe
                var establishment = await _context.Establishments
                    .FirstOrDefaultAsync(e => e.Id == dto.EstablishmentId);

                if (establishment == null)
                    return new ResponseDto(404, "Establecimiento no encontrado.");

                // Crear la nueva zona
                var zone = new Zone
                {
                    Name = dto.Name,
                    EstablishmentId = dto.EstablishmentId
                };

                // Guardar la zona en la base de datos
                _context.Zones.Add(zone);
                await _context.SaveChangesAsync();

                return new ResponseDto(201, zone, "Zona creada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateZoneAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al crear la zona.");
            }
        }

        public async Task<ResponseDto> UpdateZoneAsync(int id, CreateZoneDto dto)
        {
            try
            {
                var zone = await _context.Zones
                    .Include(z => z.Establishment)
                    .FirstOrDefaultAsync(z => z.Id == id);

                if (zone == null)
                    return new ResponseDto(404, "Zona no encontrada.");

                // Verificar si el Establishment existe
                var establishment = await _context.Establishments
                    .FirstOrDefaultAsync(e => e.Id == dto.EstablishmentId);

                if (establishment == null)
                    return new ResponseDto(404, "Establecimiento no encontrado.");

                // Actualizar los datos de la zona
                zone.Name = dto.Name;
                zone.EstablishmentId = dto.EstablishmentId;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, zone, "Zona actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateZoneAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al actualizar la zona.");
            }
        }

        public async Task<ResponseDto> DeleteZoneAsync(int id)
        {
            try
            {
                var zone = await _context.Zones.FindAsync(id);

                if (zone == null)
                    return new ResponseDto(404, "Zona no encontrada.");

                _context.Zones.Remove(zone);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, "Zona eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteZoneAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al eliminar la zona.");
            }
        }
    }
}

