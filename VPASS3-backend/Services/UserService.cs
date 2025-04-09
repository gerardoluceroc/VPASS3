using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Models;
using System.Threading.Tasks;
using System.Linq;
using VPASS3_backend.DTOs;

namespace VPASS3_backend.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;


        public UserService(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Crear usuario
        public async Task<ResponseDto> CreateUserAsync(CreateUserDto userDto)
        {
            try
            {

                // Se comprueba si ya existe un usuario con el correo electrónico
                var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
                if (existingUser != null)
                    return new ResponseDto(409, message: "El correo electrónico ya está registrado.");

                // Se busca el rol con el RoleId proporcionado
                var role = await _roleManager.FindByIdAsync(userDto.RoleId);
                if (role == null)
                    return new ResponseDto(404, message: "El rol especificado no existe.");

                // Creamos el usuario
                var user = new User
                {
                    UserName = userDto.Email,
                    Email = userDto.Email,
                };

                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (result.Succeeded)
                {
                    // Asignamos el rol al usuario
                    var roleResult = await _userManager.AddToRoleAsync(user, role.Name);
                    if (!roleResult.Succeeded)
                    {
                        return new ResponseDto(400, message: "No se pudo asignar el rol al usuario.");
                    }

                    return new ResponseDto(201, data: user.Id, message: "Usuario creado con éxito.");
                }

                return new ResponseDto(400, message: "No se pudo crear el usuario. Intente nuevamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateUserAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor.");
            }
        }

        //Obtener todos los usuarios
        public async Task<ResponseDto> GetAllUsersAsync()
        {
            try
            {
                // Obtiene todos los usuarios
                var users = await _userManager.Users.ToListAsync();
                var usersWithRoles = new List<GetUserDto>();

                foreach (var user in users)
                {
                    // Obtiene los roles asociados al usuario
                    var roleNames = await _userManager.GetRolesAsync(user);
                    var roles = new List<Role>();

                    foreach (var roleName in roleNames)
                    {
                        var role = await _roleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            roles.Add(role);  // Añade la entidad completa de Role
                        }
                    }

                    // Crea el GetUserDto con todos los atributos del usuario y los roles asociados
                    usersWithRoles.Add(new GetUserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        NormalizedUserName = user.NormalizedUserName,
                        Email = user.Email,
                        NormalizedEmail = user.NormalizedEmail,
                        EmailConfirmed = user.EmailConfirmed,
                        PasswordHash = user.PasswordHash,
                        SecurityStamp = user.SecurityStamp,
                        ConcurrencyStamp = user.ConcurrencyStamp,
                        PhoneNumber = user.PhoneNumber,
                        PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                        TwoFactorEnabled = user.TwoFactorEnabled,
                        LockoutEnd = user.LockoutEnd,
                        LockoutEnabled = user.LockoutEnabled,
                        AccessFailedCount = user.AccessFailedCount,
                        Roles = roles  // Incluye la lista de roles completos
                    });
                }

                return new ResponseDto(200, data: usersWithRoles);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener usuarios: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor.");
            }
        }

        // Obtener usuario por id
        public async Task<ResponseDto> GetUserByIdAsync(string userId)
        {
            try
            {
                // Obtiene el usuario por ID
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return new ResponseDto(404, message: "Usuario no encontrado.");
                }

                // Obtiene los roles asociados al usuario
                var roleNames = await _userManager.GetRolesAsync(user);
                var roles = new List<Role>();

                foreach (var roleName in roleNames)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        roles.Add(role);  // Añade la entidad completa de Role
                    }
                }

                // Crea el GetUserDto con todos los atributos del usuario y los roles asociados
                var userDto = new GetUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    NormalizedUserName = user.NormalizedUserName,
                    Email = user.Email,
                    NormalizedEmail = user.NormalizedEmail,
                    EmailConfirmed = user.EmailConfirmed,
                    PasswordHash = user.PasswordHash,
                    SecurityStamp = user.SecurityStamp,
                    ConcurrencyStamp = user.ConcurrencyStamp,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LockoutEnd = user.LockoutEnd,
                    LockoutEnabled = user.LockoutEnabled,
                    AccessFailedCount = user.AccessFailedCount,
                    Roles = roles  // Incluye la lista de roles completos
                };

                return new ResponseDto(200, data: userDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetUserByIdAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor.");
            }
        }

        //Actualizar usuario
        public async Task<ResponseDto> UpdateUserAsync(CreateUserDto userDto)
        {
            try
            {
                // Se busca al usuario en la base de datos utilizando el correo electrónico proporcionado.
                var existingUser = await _userManager.FindByEmailAsync(userDto.Email);

                // Si no se encuentra el usuario, se devuelve un mensaje de error indicando que no se encontró el usuario.
                if (existingUser == null)
                    return new ResponseDto(404, message: "Usuario no encontrado.");

                // Se actualiza el nombre de usuario con el correo electrónico proporcionado en el DTO.
                existingUser.UserName = userDto.Email;

                // Aquí se mantiene el rol original del usuario si no se proporciona un nuevo RoleId o si el RoleId es inválido.
                if (!string.IsNullOrEmpty(userDto.RoleId))
                {
                    // Se valida si el RoleId proporcionado existe en la base de datos.
                    var role = await _roleManager.FindByIdAsync(userDto.RoleId);

                    // Si el rol es válido, se asigna al usuario.
                    if (role != null)
                    {
                        // Primero se eliminan los roles actuales para evitar asignaciones múltiples.
                        var currentRoles = await _userManager.GetRolesAsync(existingUser);
                        await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);

                        // Se asigna el nuevo rol al usuario.
                        await _userManager.AddToRoleAsync(existingUser, role.Name);
                    }
                    // Si el rol no existe, el rol del usuario no se cambia.
                }

                // Se genera un token de restablecimiento de contraseña para actualizar la contraseña del usuario.
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);

                // Se intenta restablecer la contraseña con el token generado y la nueva contraseña proporcionada en el DTO.
                var passwordResult = await _userManager.ResetPasswordAsync(existingUser, resetToken, userDto.Password);

                // Si la actualización de la contraseña falla, se devuelve un mensaje de error.
                if (!passwordResult.Succeeded)
                    return new ResponseDto(400, message: "No se pudo actualizar la contraseña. Intente nuevamente.");

                // Se intenta actualizar la información del usuario en la base de datos.
                var updateResult = await _userManager.UpdateAsync(existingUser);

                // Si la actualización es exitosa, se devuelve un mensaje de éxito.
                return updateResult.Succeeded
                    ? new ResponseDto(200, message: "Usuario actualizado con éxito.")
                    : new ResponseDto(400, message: "No se pudo actualizar el usuario. Intente nuevamente.");
            }
            catch (Exception ex)
            {
                // Si ocurre un error inesperado, se captura y se devuelve un mensaje de error genérico con el código 500.
                Console.WriteLine("Error en UpdateUserAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor.");
            }
        }



        // Eliminar usuario
        public async Task<ResponseDto> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new ResponseDto(404, message: "Usuario no encontrado.");

                var result = await _userManager.DeleteAsync(user);

                return result.Succeeded
                    ? new ResponseDto(200, message: "Usuario eliminado con éxito.")
                    : new ResponseDto(400, message: "No se pudo eliminar el usuario. Intente nuevamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteUserAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor.");
            }
        }
    }

}
