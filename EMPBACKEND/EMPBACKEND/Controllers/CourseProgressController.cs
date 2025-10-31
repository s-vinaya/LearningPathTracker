using Microsoft.AspNetCore.Mvc;
using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseProgressController : ControllerBase
    {
        private readonly ICourseProgressService _service;

        public CourseProgressController(ICourseProgressService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseProgressDto>>> GetAll()
        {
            var progress = await _service.GetAllAsync();
            return Ok(progress);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseProgressDto>> GetById(int id)
        {
            var progress = await _service.GetByIdAsync(id);
            return progress == null ? NotFound() : Ok(progress);
        }

        [HttpGet("enrollment/{enrollmentId}")]
        public async Task<ActionResult<IEnumerable<CourseProgressDto>>> GetByEnrollmentId(int enrollmentId)
        {
            var progress = await _service.GetByEnrollmentIdAsync(enrollmentId);
            return Ok(progress);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<CourseProgressDto>>> GetByCourseId(int courseId)
        {
            var progress = await _service.GetByCourseIdAsync(courseId);
            return Ok(progress);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CourseProgressDto>> Update(int id, UpdateCourseProgressDto updateDto)
        {
            var progress = await _service.UpdateAsync(id, updateDto);
            return Ok(progress);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}