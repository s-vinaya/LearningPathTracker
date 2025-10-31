using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserAssessmentController : ControllerBase
    {
        private readonly IUserAssessmentService _service;

        public UserAssessmentController(IUserAssessmentService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> GetAllUserAssessments()
        {
            try
            {
                var userAssessments = await _service.GetAllUserAssessmentsAsync();
                return Ok(userAssessments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserAssessment(int id)
        {
            try
            {
                var userAssessment = await _service.GetUserAssessmentByIdAsync(id);
                if (userAssessment == null) return NotFound();
                return Ok(userAssessment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetUserAssessmentsByUser(int userId)
        {
            try
            {
                var userAssessments = await _service.GetUserAssessmentsByUserIdAsync(userId);
                return Ok(userAssessments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("assessment/{assessmentId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> GetUserAssessmentsByAssessment(int assessmentId)
        {
            try
            {
                var userAssessments = await _service.GetUserAssessmentsByAssessmentIdAsync(assessmentId);
                return Ok(userAssessments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateUserAssessment([FromBody] CreateUserAssessmentDto dto)
        {
            try
            {
                var createdUserAssessment = await _service.CreateUserAssessmentAsync(dto);
                return CreatedAtAction(nameof(GetUserAssessment), new { id = createdUserAssessment.Id }, createdUserAssessment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> UpdateUserAssessment(int id, [FromBody] UpdateUserAssessmentDto dto)
        {
            try
            {
                if (id != dto.Id) return BadRequest("ID mismatch");
                var updatedUserAssessment = await _service.UpdateUserAssessmentAsync(dto);
                return Ok(updatedUserAssessment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUserAssessment(int id)
        {
            try
            {
                var result = await _service.DeleteUserAssessmentAsync(id);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}