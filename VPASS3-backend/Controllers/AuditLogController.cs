using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.AuditLogs;
using VPASS3_backend.Filters;
using VPASS3_backend.Interfaces;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var response = await _auditLogService.GetAllLogsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var response = await _auditLogService.GetLogByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Actualización de log")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ResponseDto>> Update(int id, [FromBody] AuditLogDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto(400, message: "Datos inválidos. Verifica los campos ingresados."));
            }

            var response = await _auditLogService.UpdateLogAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }


        [Authorize(Policy = "ManageOwnProfile")]
        [Audit("Eliminación de log")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var response = await _auditLogService.DeleteLogAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }

}