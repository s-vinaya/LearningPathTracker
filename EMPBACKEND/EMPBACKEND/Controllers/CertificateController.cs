using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _service;

        public CertificateController(ICertificateService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<CertificateDto>>> GetAll()
        {
            var certificates = await _service.GetAllAsync();
            return Ok(certificates);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CertificateDto>> GetById(int id)
        {
            var certificate = await _service.GetByIdAsync(id);
            return certificate == null ? NotFound() : Ok(certificate);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CertificateDto>>> GetByUserId(int userId)
        {
            var certificates = await _service.GetByUserIdAsync(userId);
            return Ok(certificates);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<CertificateDto>> Create(CreateCertificateDto createDto)
        {
            var certificate = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = certificate.CertificateId }, certificate);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<CertificateDto>> Update(int id, UpdateCertificateDto updateDto)
        {
            var certificate = await _service.UpdateAsync(id, updateDto);
            return Ok(certificate);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}