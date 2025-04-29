using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.Services;

namespace VPASS3_backend.Controllers
{
    // Controlador para roles
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
                return BadRequest(new ResponseDto { StatusCode = 400, Message = "Error de validación.", Data = errores });
            }

            var response = await _roleService.CreateRoleAsync(roleDto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageEverything")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await _roleService.GetAllRolesAsync();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageEverything")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var response = await _roleService.GetRoleByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageEverything")]
        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
                return BadRequest(new ResponseDto { StatusCode = 400, Message = "Error de validación.", Data = errores });
            }

            var response = await _roleService.UpdateRoleAsync(id, roleDto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageEverything")]
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var response = await _roleService.DeleteRoleAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
