using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _service;

        public EnrollmentController(IEnrollmentService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetAll()
        {
            var enrollments = await _service.GetAllAsync();
            return Ok(enrollments);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetByUser(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && userRole != "Manager" && currentUserId != userId)
                return Forbid();

            var enrollments = await _service.GetByUserIdAsync(userId);
            return Ok(enrollments);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<EnrollmentDto>> Create(CreateEnrollmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var enrollment = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, enrollment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentDto>> GetById(int id)
        {
            var enrollment = await _service.GetByIdAsync(id);
            if (enrollment == null) return NotFound();
            return Ok(enrollment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EnrollmentDto>> Update(int id, UpdateEnrollmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var enrollment = await _service.UpdateAsync(id, dto);
                return Ok(enrollment);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}