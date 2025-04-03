using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Context;
using VPASS3_backend.Models;

namespace VPASS3_backend.Services
{
    public class RoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        // Obtener todos los roles
        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        // Obtener un rol por ID
        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        // Crear un nuevo rol
        public async Task<Role> CreateRoleAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        // Actualizar un rol
        public async Task<bool> UpdateRoleAsync(int id, Role role)
        {
            if (id != role.Id)
                return false;

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                    return false;

                throw;
            }
        }

        // Eliminar un rol
        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return false;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        // Verificar si un rol existe
        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
