using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces;
using EMPBACKEND.Interfaces.Services;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssessmentController : ControllerBase
    {
        private readonly IAssessmentService _service;

        public AssessmentController(IAssessmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssessmentDto>>> GetAll()
        {
            var assessments = await _service.GetAllAsync();
            return Ok(assessments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssessmentDto>> GetById(int id)
        {
            var assessment = await _service.GetByIdAsync(id);
            return assessment == null ? NotFound() : Ok(assessment);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<AssessmentDto>>> GetByCourseId(int courseId)
        {
            var assessments = await _service.GetByCourseIdAsync(courseId);
            return Ok(assessments);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<AssessmentDto>> Create(CreateAssessmentDto createDto)
        {
            var assessment = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = assessment.Id }, assessment);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<AssessmentDto>> Update(int id, UpdateAssessmentDto updateDto)
        {
            var assessment = await _service.UpdateAsync(id, updateDto);
            return Ok(assessment);
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