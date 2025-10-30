using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<ActionResult<IEnumerable<UserAssessmentDto>>> GetAll()
        {
            var userAssessments = await _service.GetAllAsync();
            return Ok(userAssessments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserAssessmentDto>> GetById(int id)
        {
            var userAssessment = await _service.GetByIdAsync(id);
            if (userAssessment == null) return NotFound();
            return Ok(userAssessment);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserAssessmentDto>>> GetByUserId(int userId)
        {
            var userAssessments = await _service.GetByUserIdAsync(userId);
            return Ok(userAssessments);
        }

        [HttpPost]
        public async Task<ActionResult<UserAssessmentDto>> Create(CreateUserAssessmentDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userAssessment = await _service.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = userAssessment.Id }, userAssessment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserAssessmentDto>> Update(int id, UpdateUserAssessmentDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userAssessment = await _service.UpdateAsync(id, updateDto);
                return Ok(userAssessment);
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
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}