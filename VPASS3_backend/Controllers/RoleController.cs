using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.Models;
using VPASS3_backend.Services;

namespace VPASS3_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        // Inyección de dependencias para el servicio de roles
        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/Role
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            var roles = await _roleService.GetRolesAsync();
            return Ok(roles);
        }

        // GET: api/Role/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        // PUT: api/Role/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, Role role)
        {
            var success = await _roleService.UpdateRoleAsync(id, role);
            if (!success)
            {
                return BadRequest();
            }

            return NoContent();
        }

        // POST: api/Role
        [HttpPost]
        public async Task<ActionResult<Role>> PostRole(Role role)
        {
            var createdRole = await _roleService.CreateRoleAsync(role);
            return CreatedAtAction("GetRole", new { id = createdRole.Id }, createdRole);
        }

        // DELETE: api/Role/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var success = await _roleService.DeleteRoleAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
