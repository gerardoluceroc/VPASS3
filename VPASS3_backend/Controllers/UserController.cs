using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Context;
using VPASS3_backend.DTOs;
using VPASS3_backend.Models;
using VPASS3_backend.Services;

namespace VPASS3_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        // Inyección de dependencias para el servicio de usuario
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // Nuevo endpoint para obtener usuarios por RoleId
        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(int roleId)
        {
            var users = await _userService.GetUsersByRoleAsync(roleId);

            if (users == null || !users.Any())
            {
                return NotFound();
            }

            return Ok(users);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Devuelve un BadRequest si el modelo no es válido
            }

            var success = await _userService.UpdateUserAsync(id, user);
            if (!success)
            {
                return BadRequest();
            }

            return NoContent();
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserDTO userDTO)
        {

            try
            {
                // Llama al servicio para crear el usuario
                var createdUser = await _userService.CreateUserAsync(userDTO);

                // Si todo está bien, se retorna un código 201 Created
                return CreatedAtAction("GetUser", new { id = createdUser.Id }, createdUser);
            }
            catch (InvalidOperationException ex)
            {
                // Si el email ya está registrado
                return Conflict(new { message = ex.Message }); // Código de estado 409 (Conflict)
            }
            catch (ArgumentException ex)
            {
                // Si no se encuentra un roleId en la solicitud
                return BadRequest(new { message = ex.Message }); // En este caso, el mensaje será "El roleId es obligatorio."
            }
            catch (KeyNotFoundException ex)
            {
                // Si no se encuentra el rol en la base de datos
                return NotFound(new { message = ex.Message }); // En este caso, el mensaje será "Rol no encontrado."
            }
            catch (Exception)
            {
                // Si ocurre un error general (cualquier otro error), devuelve un Error de servidor (500) con un mensaje genérico
                return StatusCode(500, new { message = "Error en el servidor" }); // 500 Internal Server Error
            }
        }



        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

