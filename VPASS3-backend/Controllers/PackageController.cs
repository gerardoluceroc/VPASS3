using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.DTOs.PackagesDtos;

namespace VPASS3_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _pkgService;
        private readonly IUserContextService _userContext;

        public PackageController(IPackageService pkgService, IUserContextService userContext)
        {
            _pkgService = pkgService;
            _userContext = userContext;
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("create")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreatePackageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));
            var resp = await _pkgService.CreateAsync(dto);
            return StatusCode(resp.StatusCode, resp);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPost("export/excel/byDates")]
        public async Task<IActionResult> ExportByDates([FromBody] GetPackagesByDatesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Parámetros inválidos."));

            var response = await _pkgService.ExportPackagesToExcelByDatesAsync(dto);
            if (response.StatusCode != 200) return StatusCode(response.StatusCode, response);

            var file = (dynamic)response.Data!;
            return File((byte[])file.FileContent, file.ContentType, file.FileName);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("all")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            var resp = await _pkgService.GetAllAsync();
            return StatusCode(resp.StatusCode, resp);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            var resp = await _pkgService.GetByIdAsync(id);
            return StatusCode(resp.StatusCode, resp);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpGet("export/excel/all")]
        public async Task<IActionResult> ExportAll()
        {
            var response = await _pkgService.ExportAllPackagesToExcelAsync();

            if (response.StatusCode != 200)
                return StatusCode(response.StatusCode, response);

            var file = (dynamic)response.Data!;
            return File((byte[])file.FileContent, file.ContentType, file.FileName);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("receive")]
        public async Task<ActionResult<ResponseDto>> ReceivePackage([FromBody] ReceivePackageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));

            var resp = await _pkgService.MarkAsDeliveredAsync(dto);
            return StatusCode(resp.StatusCode, resp);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpPut("update")]
        public async Task<ActionResult<ResponseDto>> Update([FromBody] UpdatePackageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto(400, message: "Datos inválidos."));
            var resp = await _pkgService.UpdateAsync(dto);
            return StatusCode(resp.StatusCode, resp);
        }

        [Authorize(Policy = "ManageOwnProfile")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var resp = await _pkgService.DeleteAsync(id);
            return StatusCode(resp.StatusCode, resp);
        }
    }
}