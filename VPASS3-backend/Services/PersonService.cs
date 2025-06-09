using VPASS3_backend.Context;
using VPASS3_backend.DTOs.Persons;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Models;

namespace VPASS3_backend.Services
{
    public class PersonService : IPersonService
    {
        private readonly AppDbContext _context;

        public PersonService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> GetAllAsync()
        {
            try
            {
                var persons = await _context.Persons.ToListAsync();
                return new ResponseDto(200, persons, "Personas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllAsync PersonService: " + ex.Message);
                return new ResponseDto(500, message: "Error al obtener las personas.");
            }
        }

        public async Task<ResponseDto> GetByIdAsync(int id)
        {
            try
            {
                var person = await _context.Persons.FindAsync(id);

                if (person == null)
                    return new ResponseDto(404, message: "Persona no encontrada.");

                return new ResponseDto(200, person, "Persona obtenida correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetByIdAsync PersonService: " + ex.Message);
                return new ResponseDto(500, message: "Error al obtener la persona.");
            }
        }

        public async Task<ResponseDto> CreateAsync(CreatePersonDto dto)
        {
            try
            {
                var exists = await _context.Persons
                    .AnyAsync(p => p.IdentificationNumber == dto.IdentificationNumber);

                if (exists)
                    return new ResponseDto(409, message: "Ya existe una persona con este número de identificación.");

                var person = new Person
                {
                    Names = dto.Names,
                    LastNames = dto.LastNames,
                    IdentificationNumber = dto.IdentificationNumber
                };

                _context.Persons.Add(person);
                await _context.SaveChangesAsync();

                return new ResponseDto(201, person, "Persona creada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateAsync PersonService: " + ex.Message);
                return new ResponseDto(500, message: "Error al crear la persona.");
            }
        }

        public async Task<ResponseDto> UpdateAsync(int id, CreatePersonDto dto)
        {
            try
            {
                var person = await _context.Persons.FindAsync(id);

                if (person == null)
                    return new ResponseDto(404, message: "Persona no encontrada.");

                var duplicate = await _context.Persons
                    .AnyAsync(p => p.Id != id && p.IdentificationNumber == dto.IdentificationNumber);

                if (duplicate)
                    return new ResponseDto(409, message: "Ya existe otra persona con ese número de identificación.");

                person.Names = dto.Names;
                person.LastNames = dto.LastNames;
                person.IdentificationNumber = dto.IdentificationNumber;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, person, "Persona actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateAsync PersonService: " + ex.Message);
                return new ResponseDto(500, message: "Error al actualizar la persona.");
            }
        }

        public async Task<ResponseDto> DeleteAsync(int id)
        {
            try
            {
                var person = await _context.Persons.FindAsync(id);

                if (person == null)
                    return new ResponseDto(404, message: "Persona no encontrada.");

                _context.Persons.Remove(person);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, message: "Persona eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteAsync PersonService: " + ex.Message);
                return new ResponseDto(500, message: "Error al eliminar la persona.");
            }
        }
    }
}
