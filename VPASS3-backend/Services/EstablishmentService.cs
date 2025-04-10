using VPASS3_backend.Context;
using VPASS3_backend.DTOs.Establishments;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;
using Microsoft.EntityFrameworkCore;


namespace VPASS3_backend.Services
{
    public class EstablishmentService : IEstablishmentService
    {
        private readonly AppDbContext _context;

        public EstablishmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> GetAllEstablishmentsAsync()
        {
            try
            {
                var establishments = await _context.Establishments
                    .Include(e => e.Users)
                    .Include(e => e.Zones)
                    .ToListAsync();

                return new ResponseDto(200, establishments, "Establecimientos obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllEstablishmentsAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener los establecimientos.");
            }
        }

        public async Task<ResponseDto> GetEstablishmentByIdAsync(int id)
        {
            try
            {
                var establishment = await _context.Establishments
                    .Include(e => e.Users)
                    .Include(e => e.Zones)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (establishment == null)
                    return new ResponseDto(404, "Establecimiento no encontrado.");

                return new ResponseDto(200, establishment, "Establecimiento obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetEstablishmentByIdAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al obtener el establecimiento.");
            }
        }

        public async Task<ResponseDto> CreateEstablishmentAsync(CreateEstablishmentDto dto)
        {
            try
            {
                // Buscar el usuario por el correo electrónico
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

                // Si no se encuentra el usuario, devolver un error
                if (user == null)
                    return new ResponseDto(404, $"No se encontró ningún usuario con el email ingresado.");

                // Comprobar si el usuario ya está asociado a un Establishment
                if (user.EstablishmentId != null)
                {
                    return new ResponseDto(409, "El usuario ya está asociado a un establecimiento y no puede ser asignado a otro.");
                }

                // Crear el nuevo establecimiento
                var establishment = new Establishment
                {
                    Name = dto.Name,
                    Users = [user]
                };

                // Asignar el nuevo establecimiento al usuario
                user.EstablishmentId = establishment.Id;
                user.establishment = establishment;

                // Guardar los cambios en la base de datos
                _context.Establishments.Add(establishment);
                await _context.SaveChangesAsync();

                return new ResponseDto(201, establishment, "Establecimiento creado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateEstablishmentAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al crear el establecimiento.");
            }
        }

        public async Task<ResponseDto> UpdateEstablishmentAsync(int id, CreateEstablishmentDto dto)
        {
            try
            {
                var establishment = await _context.Establishments
                    .Include(e => e.Users)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (establishment == null)
                    return new ResponseDto(404, "Establecimiento no encontrado.");

                establishment.Name = dto.Name;

                if (!string.IsNullOrEmpty(dto.Email))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

                    if (user == null)
                        return new ResponseDto(404, $"No se encontró ningún usuario con el email ingresado");

                    // Verificar si el usuario ya está asociado a otro establecimiento
                    if (user.EstablishmentId != null && user.EstablishmentId != establishment.Id)
                    {
                        return new ResponseDto(409, "El usuario ya está asociado a otro establecimiento.");
                    }

                    // Limpiar la relación anterior
                    foreach (var u in establishment.Users)
                    {
                        u.EstablishmentId = null;
                        u.establishment = null;
                    }

                    establishment.Users = [user];
                    user.EstablishmentId = establishment.Id;
                    user.establishment = establishment;
                }

                await _context.SaveChangesAsync();
                return new ResponseDto(200, establishment, "Establecimiento actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateEstablishmentAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al actualizar el establecimiento.");
            }
        }

        public async Task<ResponseDto> DeleteEstablishmentAsync(int id)
        {
            try
            {
                var establishment = await _context.Establishments.FindAsync(id);

                if (establishment == null)
                    return new ResponseDto(404, "Establecimiento no encontrado.");

                _context.Establishments.Remove(establishment);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, "Establecimiento eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteEstablishmentAsync: " + ex.Message);
                return new ResponseDto(500, "Error en el servidor al eliminar el establecimiento.");
            }
        }
    }
}












































//namespace VPASS3_backend.Services
//{
//    public class EstablishmentService : IEstablishmentService
//    {
//        private readonly AppDbContext _context;

//        public EstablishmentService(AppDbContext context)
//        {
//            _context = context;
//        }

//        // Crear Establecimineto
//        public async Task<ResponseDto> CreateEstablishmentAsync(CreateEstablishmentDto createEstablishmentDto)
//        {
//            try
//            {
//                // Verificar si ya existe un establecimiento con ese nombre
//                var exists = await _context.Establishments.AnyAsync(e => e.Name == createEstablishmentDto.Name);
//                if (exists)
//                {
//                    return new ResponseDto(409, message: "Ya existe un establecimiento con ese nombre.");
//                }

//                // Buscar el usuario por email
//                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == createEstablishmentDto.Email);
//                if (user == null)
//                {
//                    return new ResponseDto(404, message: "No se encontró un usuario con ese email.");
//                }

//                // Crear establecimiento y asignar el usuario
//                var establishment = new Establishment
//                {
//                    Name = createEstablishmentDto.Name,
//                    Users = [user] // Asociar el usuario
//                };

//                await _context.Establishments.AddAsync(establishment);
//                await _context.SaveChangesAsync();

//                return new ResponseDto(201, message: "Establecimiento creado y asociado al usuario correctamente.", data: establishment);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error en CreateEstablishmentAsync: " + ex.Message);
//                return new ResponseDto(500, message: "Error en el servidor al crear el establecimiento.");
//            }
//        }


//        // Obtener todos los establecimientos
//        public async Task<ResponseDto> GetAllEstablishmentsAsync()
//        {
//            try
//            {
//                //// Obtener los establecimientos, incluyendo los usuarios
//                //var establishments = await _context.Establishments
//                //    .Include(e => e.Users)
//                //    .ToListAsync();

//                return new ResponseDto(200, message: "Establecimientos obtenidos correctamente.", data: establishments);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error en GetAllEstablishmentsAsync: " + ex.Message);
//                return new ResponseDto(500, message: "Error en el servidor al obtener los establecimientos.");
//            }
//        }


//        // Obtener establecimiento por id
//        public async Task<ResponseDto> GetEstablishmentByIdAsync(int id)
//        {
//            try
//            {
//                var establishment = await _context.Establishments.FindAsync(id);
//                if (establishment == null)
//                {
//                    return new ResponseDto(404, message: "Establecimiento no encontrado.");
//                }

//                return new ResponseDto(200, message: "Establecimiento obtenido correctamente.", data: establishment);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error en GetEstablishmentByIdAsync: " + ex.Message);
//                return new ResponseDto(500, message: "Error en el servidor al obtener el establecimiento.");
//            }
//        }

//        // Actualizar establecimiento
//        public async Task<ResponseDto> UpdateEstablishmentAsync(int id, CreateEstablishmentDto createEstablishmentDto)
//        {
//            try
//            {
//                // Buscar el establecimiento por ID
//                var establishment = await _context.Establishments.Include(e => e.Users).FirstOrDefaultAsync(e => e.Id == id);
//                if (establishment == null)
//                {
//                    return new ResponseDto(404, message: "Establecimiento no encontrado.");
//                }

//                // Si se proporciona un email y existe el usuario con ese email, lo asociamos al establecimiento
//                if (!string.IsNullOrEmpty(createEstablishmentDto.Email))
//                {
//                    var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == createEstablishmentDto.Email);
//                    if (user != null)
//                    {
//                        establishment.Users = new List<User> { user };
//                    }
//                }

//                // Si se proporciona un nombre, actualizamos el nombre del establecimiento
//                if (!string.IsNullOrEmpty(createEstablishmentDto.Name))
//                {
//                    establishment.Name = createEstablishmentDto.Name;
//                }

//                // Si no se proporcionan ni email ni nombre, se mantienen los valores originales

//                // Marcar el establecimiento para ser actualizado
//                _context.Establishments.Update(establishment);
//                await _context.SaveChangesAsync();

//                return new ResponseDto(200, message: "Establecimiento actualizado correctamente.", data: establishment);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error en UpdateEstablishmentAsync: " + ex.Message);
//                return new ResponseDto(500, message: "Error en el servidor al actualizar el establecimiento.");
//            }
//        }


//        // Borrrar establecimiento
//        public async Task<ResponseDto> DeleteEstablishmentAsync(int id)
//        {
//            try
//            {
//                var establishment = await _context.Establishments.FindAsync(id);
//                if (establishment == null)
//                {
//                    return new ResponseDto(404, message: "Establecimiento no encontrado.");
//                }

//                _context.Establishments.Remove(establishment);
//                await _context.SaveChangesAsync();

//                return new ResponseDto(200, message: "Establecimiento eliminado correctamente.");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error en DeleteEstablishmentAsync: " + ex.Message);
//                return new ResponseDto(500, message: "Error en el servidor al eliminar el establecimiento.");
//            }
//        }
//    }


//}
