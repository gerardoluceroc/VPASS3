using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Context;
using VPASS3_backend.DTOs;
using VPASS3_backend.Models;

namespace VPASS3_backend.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Obtener todos los usuarios con su rol asociado
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            // Incluir el rol asociado al usuario en la consulta
            return await _context.Users.Include(u => u.Role).ToListAsync();
        }

        // Obtener un usuario por ID con su rol asociado
        public async Task<User> GetUserByIdAsync(int id)
        {
            // Incluir el rol asociado al usuario en la consulta
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
        }

        // Obtener todos los usuarios por un RoleId específico
        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
        {
            return await _context.Users
                                 .Where(u => u.RoleId == roleId)
                                 .ToListAsync();
        }

        public async Task<User> CreateUserAsync(UserDTO userDTO)
        {
            // Verifica si el email ya está registrado
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userDTO.Email);

            if (existingUser != null)
            {
                // Lanza una excepción si el email ya está registrado
                throw new InvalidOperationException("El correo electrónico ya está registrado.");
            }

            // Verifica si el rol existe en la base de datos
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == userDTO.RoleId);

            if (role == null)
            {
                throw new KeyNotFoundException("Rol no encontrado con el ID proporcionado.");
            }

            // Crear un nuevo usuario con los datos del DTO
            var user = new User
            {
                Email = userDTO.Email,
                Password = userDTO.Password,
                RoleId = userDTO.RoleId,
                Role = role // Asociamos el rol encontrado
            };

            // Agrega el usuario a la base de datos
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }



        // Actualizar un usuario
        public async Task<bool> UpdateUserAsync(int id, User user)
        {
            if (id != user.Id)
                return false;

            // Si quieres permitir que el rol sea modificado, puedes dejarlo así
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                    return false;

                throw;
            }
        }

        // Eliminar un usuario
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Verificar si un usuario existe
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}


