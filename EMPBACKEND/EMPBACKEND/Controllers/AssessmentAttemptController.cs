using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssessmentAttemptController : ControllerBase
    {
        private readonly IAssessmentAttemptService _service;

        public AssessmentAttemptController(IAssessmentAttemptService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<AssessmentAttemptDto>>> GetAll()
        {
            var attempts = await _service.GetAllAsync();
            return Ok(attempts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssessmentAttemptDto>> GetById(int id)
        {
            var attempt = await _service.GetByIdAsync(id);
            return attempt == null ? NotFound() : Ok(attempt);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AssessmentAttemptDto>>> GetByUserId(int userId)
        {
            var attempts = await _service.GetByUserIdAsync(userId);
            return Ok(attempts);
        }

        [HttpGet("assessment/{assessmentId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<AssessmentAttemptDto>>> GetByAssessmentId(int assessmentId)
        {
            var attempts = await _service.GetByAssessmentIdAsync(assessmentId);
            return Ok(attempts);
        }

        [HttpPost]
        public async Task<ActionResult<AssessmentAttemptDto>> Create(CreateAssessmentAttemptDto createDto)
        {
            var attempt = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = attempt.AttemptId }, attempt);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}