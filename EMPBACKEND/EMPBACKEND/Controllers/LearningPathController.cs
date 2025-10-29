using Microsoft.AspNetCore.Mvc;
using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearningPathController : ControllerBase
    {
        private readonly ILearningPathService _service;

        public LearningPathController(ILearningPathService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LearningPathDto>>> GetAll()
        {
            var paths = await _service.GetAllAsync();
            return Ok(paths);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LearningPathDto>> GetById(int id)
        {
            var path = await _service.GetByIdAsync(id);
            return path == null ? NotFound() : Ok(path);
        }

        [HttpGet("{id}/courses")]
        public async Task<ActionResult<IEnumerable<LearningPathCourseDto>>> GetLearningPathCourses(int id)
        {
            var courses = await _service.GetLearningPathCoursesAsync(id);
            return Ok(courses);
        }

        [HttpPost]
        public async Task<ActionResult<LearningPathDto>> Create(CreateLearningPathDto createDto)
        {
            var path = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = path.Id }, path);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LearningPathDto>> Update(int id, UpdateLearningPathDto updateDto)
        {
            var path = await _service.UpdateAsync(id, updateDto);
            return Ok(path);
        }

        [HttpPost("{pathId}/courses")]
        public async Task<ActionResult<LearningPathCourseDto>> AddCourseToPath(int pathId, CreateLearningPathCourseDto courseDto)
        {
            var pathCourse = await _service.AddCourseToPathAsync(pathId, courseDto);
            return Ok(pathCourse);
        }

        [HttpDelete("{pathId}/courses/{courseId}")]
        public async Task<ActionResult> RemoveCourseFromPath(int pathId, int courseId)
        {
            var result = await _service.RemoveCourseFromPathAsync(pathId, courseId);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}