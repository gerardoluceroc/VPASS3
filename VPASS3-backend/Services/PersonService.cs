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
        private readonly IUserContextService _userContext;

        public PersonService(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<ResponseDto> GetAllPersonsAsync()
        {
            try
            {
                var persons = await _context.Persons.ToListAsync();

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    persons = persons
                        .Where(p => _userContext.CanAccessPerson(p))
                        .ToList();
                }

                return new ResponseDto(200, persons, "Personas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllPersonsAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener las personas.");
            }
        }

        public async Task<ResponseDto> GetPersonByIdAsync(int id)
        {
            try
            {
                var person = await _context.Persons
                    .Include(p => p.InvitedCommonAreaReservations)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (person == null)
                    return new ResponseDto(404, message: "Persona no encontrada.");

                if (!_userContext.CanAccessPerson(person))
                    return new ResponseDto(403, message: "No tienes permiso para acceder a esta persona.");

                return new ResponseDto(200, person, "Persona obtenida correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetPersonByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener la persona.");
            }
        }

        public async Task<ResponseDto> GetPersonByIdentificationNumberAsync(string identificationNumber)
        {
            try
            {
                var person = await _context.Persons
                    .Include(p => p.InvitedCommonAreaReservations)
                    .FirstOrDefaultAsync(p => p.IdentificationNumber == identificationNumber);

                if (person == null)
                    return new ResponseDto(404, message: "Persona no encontrada.");

                if (!_userContext.CanAccessPerson(person))
                    return new ResponseDto(403, message: "No tienes permiso para acceder a esta persona.");

                return new ResponseDto(200, person, "Persona obtenida correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetPersonByIdentificationNumberAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener la persona.");
            }
        }

        public async Task<ResponseDto> CreatePersonAsync(PersonDto dto)
        {
            try
            {
                var exists = await _context.Persons
                    .AnyAsync(p => p.IdentificationNumber == dto.IdentificationNumber);

                if (exists)
                    return new ResponseDto(409, message: "Ya existe una persona con ese número de identificación.");

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
                Console.WriteLine("Error en CreatePersonAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al crear la persona.");
            }
        }

        public async Task<ResponseDto> UpdatePersonAsync(int id, PersonDto dto)
        {
            try
            {
                var person = await _context.Persons
                    .Include(p => p.InvitedCommonAreaReservations)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (person == null)
                    return new ResponseDto(404, message: "Persona no encontrada.");

                if (!_userContext.CanAccessPerson(person))
                    return new ResponseDto(403, message: "No tienes permiso para modificar esta persona.");

                var duplicate = await _context.Persons
                    .AnyAsync(p => p.IdentificationNumber == dto.IdentificationNumber && p.Id != id);

                if (duplicate)
                    return new ResponseDto(400, message: "Ya existe otra persona con ese número de identificación.");

                person.Names = dto.Names;
                person.LastNames = dto.LastNames;
                person.IdentificationNumber = dto.IdentificationNumber;

                await _context.SaveChangesAsync();

                return new ResponseDto(200, person, "Persona actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdatePersonAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al actualizar la persona.");
            }
        }

        public async Task<ResponseDto> DeletePersonAsync(int id)
        {
            try
            {
                var person = await _context.Persons
                    .Include(p => p.InvitedCommonAreaReservations)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (person == null)
                    return new ResponseDto(404, message: "Persona no encontrada.");

                if (!_userContext.CanAccessPerson(person))
                    return new ResponseDto(403, message: "No tienes permiso para eliminar esta persona.");

                if (person.InvitedCommonAreaReservations.Any())
                    return new ResponseDto(400, message: "No se puede eliminar la persona porque tiene reservas asociadas como invitado.");

                _context.Persons.Remove(person);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, message: "Persona eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeletePersonAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar la persona.");
            }
        }
    }
}
