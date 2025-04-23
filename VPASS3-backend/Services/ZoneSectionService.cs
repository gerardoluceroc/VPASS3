using VPASS3_backend.Context;
using VPASS3_backend.DTOs.ZoneSections;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace VPASS3_backend.Services
{
    public class ZoneSectionService : IZoneSectionService
    {
        private readonly AppDbContext _context;

        public ZoneSectionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> GetAllZoneSectionsAsync()
        {
            try
            {
                var sections = await _context.ZoneSections
                    .Include(zs => zs.Zone)
                    .ToListAsync();

                return new ResponseDto(200, sections, "Secciones obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllZoneSectionsAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener las secciones.");
            }
        }

        public async Task<ResponseDto> GetZoneSectionByIdAsync(int id)
        {
            try
            {
                var section = await _context.ZoneSections
                    .Include(zs => zs.Zone)
                    .FirstOrDefaultAsync(zs => zs.Id == id);

                if (section == null)
                    return new ResponseDto(404, "Sección no encontrada.");

                return new ResponseDto(200, section, "Sección obtenida correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetZoneSectionByIdAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener la sección.");
            }
        }

        public async Task<ResponseDto> CreateZoneSectionAsync(ZoneSectionDto dto)
        {
            try
            {
                var zone = await _context.Zones.FindAsync(dto.IdZone);
                if (zone == null)
                    return new ResponseDto(404, "Zona asociada no encontrada.");

                var zoneSection = new ZoneSection
                {
                    Name = dto.Name,
                    IdZone = dto.IdZone
                };

                _context.ZoneSections.Add(zoneSection);
                await _context.SaveChangesAsync();

                return new ResponseDto(201, zoneSection, "Sección creada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateZoneSectionAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al crear la sección.");
            }
        }

        public async Task<ResponseDto> UpdateZoneSectionAsync(int id, ZoneSectionDto dto)
        {
            try
            {
                var section = await _context.ZoneSections.FindAsync(id);
                if (section == null)
                    return new ResponseDto(404, "Sección no encontrada.");

                var zone = await _context.Zones.FindAsync(dto.IdZone);
                if (zone == null)
                    return new ResponseDto(404, "Zona asociada no encontrada.");

                section.Name = dto.Name;
                section.IdZone = dto.IdZone;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, section, "Sección actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateZoneSectionAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al actualizar la sección.");
            }
        }

        public async Task<ResponseDto> DeleteZoneSectionAsync(int id)
        {
            try
            {
                var section = await _context.ZoneSections.FindAsync(id);
                if (section == null)
                    return new ResponseDto(404, "Sección no encontrada.");

                _context.ZoneSections.Remove(section);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, "Sección eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteZoneSectionAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al eliminar la sección.");
            }
        }
    }
}
