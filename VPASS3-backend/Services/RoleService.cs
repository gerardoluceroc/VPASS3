using Microsoft.AspNetCore.Identity;
using VPASS3_backend.DTOs;
using VPASS3_backend.Models;

namespace VPASS3_backend.Services
{
    // Servicio para gestionar roles
    public class RoleService
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleService(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        // Crear rol
        public async Task<ResponseDto> CreateRoleAsync(RoleDto roleDto)
        {
            try
            {
                // Verifica si el rol ya existe
                var existingRole = await _roleManager.FindByNameAsync(roleDto.Name);
                if (existingRole != null)
                    return new ResponseDto(409, message: "El rol ya existe.");

                // Crea una nueva instancia del rol utilizando tu clase personalizada 'Role'
                var newRole = new Role
                {
                    Name = roleDto.Name
                };

                // Crea el rol usando el RoleManager
                var result = await _roleManager.CreateAsync(newRole);

                // Retorna la respuesta según si la creación fue exitosa o no
                return result.Succeeded
                    ? new ResponseDto(201, data: newRole.Id, message: "Rol creado con éxito.")
                    : new ResponseDto(400, message: "No se pudo crear el rol.");
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine("Error al crear rol: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor.");
            }
        }


        // Obtener todos los roles
        public async Task<ResponseDto> GetAllRolesAsync()
        {
            try
            {
                var roles = _roleManager.Roles.ToList();
                return new ResponseDto { StatusCode = 200, Data = roles };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener roles: " + ex.Message);
                return new ResponseDto { StatusCode = 500, Message = "Error en el servidor." };
            }
        }

        // Obtener rol por id
        public async Task<ResponseDto> GetRoleByIdAsync(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                return role != null
                    ? new ResponseDto { StatusCode = 200, Data = role }
                    : new ResponseDto { StatusCode = 404, Message = "Rol no encontrado." };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener rol por ID: " + ex.Message);
                return new ResponseDto { StatusCode = 500, Message = "Error en el servidor." };
            }
        }

        // Actualizar rol
        public async Task<ResponseDto> UpdateRoleAsync(string id, RoleDto roleDto)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                    return new ResponseDto { StatusCode = 404, Message = "Rol no encontrado." };

                role.Name = roleDto.Name;
                var result = await _roleManager.UpdateAsync(role);

                return result.Succeeded
                    ? new ResponseDto { StatusCode = 200, Message = "Rol actualizado con éxito." }
                    : new ResponseDto { StatusCode = 400, Message = "No se pudo actualizar el rol." };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar rol: " + ex.Message);
                return new ResponseDto { StatusCode = 500, Message = "Error en el servidor." };
            }
        }

        // Eliminar rol
        public async Task<ResponseDto> DeleteRoleAsync(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                    return new ResponseDto { StatusCode = 404, Message = "Rol no encontrado." };

                var result = await _roleManager.DeleteAsync(role);

                return result.Succeeded
                    ? new ResponseDto { StatusCode = 200, Message = "Rol eliminado con éxito." }
                    : new ResponseDto { StatusCode = 400, Message = "No se pudo eliminar el rol." };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar rol: " + ex.Message);
                return new ResponseDto { StatusCode = 500, Message = "Error en el servidor." };
            }
        }
    }
}

