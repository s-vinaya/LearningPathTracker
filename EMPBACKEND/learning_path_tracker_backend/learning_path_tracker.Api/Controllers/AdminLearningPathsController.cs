using learning_path_tracker.Application.DTOs.LearningPaths;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Application.Logging;
using learning_path_tracker.Api.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize]
[AuthorizeRoles("Admin")]
public class AdminLearningPathsController : ControllerBase
{
    private readonly ILearningPathService _learningPathService;
    private readonly FileLogger _logger;

    public AdminLearningPathsController(ILearningPathService learningPathService, FileLogger logger)
    {
        _learningPathService = learningPathService;
        _logger = logger;
    }

    [HttpGet("learning-paths")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            _logger.LogInfo("Controller", "GetAll learning paths called");
            var learningPaths = await _learningPathService.GetAllLearningPathsAsync();
            return Ok(learningPaths);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetAll learning paths failed", ex);
            return StatusCode(500, new { message = "Error retrieving learning paths" });
        }
    }

    [HttpPost("learning-paths")]
    public async Task<IActionResult> Create([FromBody] CreateLearningPathDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                _logger.LogWarning("Controller", "Create learning path: Invalid input");
                return BadRequest(new { message = "Title is required" });
            }

            _logger.LogInfo("Controller", $"Create learning path: {dto.Title}");
            var learningPath = await _learningPathService.CreateLearningPathAsync(dto);
            _logger.LogInfo("Controller", $"Learning path created: pathId={learningPath.Id}");
            return CreatedAtAction(nameof(GetAll), new { id = learningPath.Id }, learningPath);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "Create learning path failed", ex);
            return StatusCode(500, new { message = "Error creating learning path" });
        }
    }

    [HttpPut("learning-paths/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLearningPathDto dto)
    {
        var learningPath = await _learningPathService.UpdateLearningPathAsync(id, dto);
        if (learningPath == null) return NotFound();
        return Ok(learningPath);
    }

    [HttpDelete("learning-paths/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInfo("Controller", $"Delete learning path: pathId={id}");
            var result = await _learningPathService.DeleteLearningPathAsync(id);
            if (!result)
            {
                _logger.LogWarning("Controller", $"Delete: Learning path not found - pathId={id}");
                return NotFound(new { message = "Learning path not found" });
            }
            _logger.LogInfo("Controller", $"Learning path deleted: pathId={id}");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"Delete learning path failed: pathId={id}", ex);
            return StatusCode(500, new { message = "Error deleting learning path" });
        }
    }

    [HttpPut("learning-paths/{id}/modules")]
    public async Task<IActionResult> UpdateModules(int id, [FromBody] List<CreateModuleDto> modules)
    {
        var result = await _learningPathService.UpdateModulesAsync(id, modules);
        if (!result) return NotFound();
        return Ok();
    }

    [HttpPost("learning-paths/{id}/modules")]
    public async Task<IActionResult> AddModule(int id, [FromBody] CreateModuleDto dto)
    {
        var module = await _learningPathService.AddModuleAsync(id, dto);
        return CreatedAtAction(nameof(GetAll), new { id = module.Id }, module);
    }

    [HttpPut("modules/{id}")]
    public async Task<IActionResult> UpdateModule(int id, [FromBody] UpdateModuleDto dto)
    {
        var module = await _learningPathService.UpdateModuleAsync(id, dto);
        if (module == null) return NotFound();
        return Ok(module);
    }

    [HttpDelete("modules/{id}")]
    public async Task<IActionResult> DeleteModule(int id)
    {
        var result = await _learningPathService.DeleteModuleAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpGet("learning-paths/statistics")]
    public async Task<IActionResult> GetLearningPathStatistics()
    {
        var stats = await _learningPathService.GetStatisticsAsync();
        return Ok(stats);
    }

    [HttpGet("learning-paths/with-courses")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLearningPathsWithCourses()
    {
        var learningPaths = await _learningPathService.GetLearningPathsWithCoursesAsync();
        return Ok(learningPaths);
    }

    [HttpPost("learning-paths/with-courses")]
    public async Task<IActionResult> CreateLearningPathWithCourses([FromBody] CreateLearningPathWithCoursesDto dto)
    {
        var learningPath = await _learningPathService.CreateLearningPathWithCoursesAsync(dto);
        return CreatedAtAction(nameof(GetLearningPathsWithCourses), learningPath);
    }

    [HttpPost("learning-paths/add-courses")]
    public async Task<IActionResult> AddCoursesToPath([FromBody] AddCoursesToPathDto dto)
    {
        var result = await _learningPathService.AddCoursesToPathAsync(dto);
        if (!result) return NotFound();
        return Ok();
    }

    [HttpPut("learning-paths/course-order")]
    public async Task<IActionResult> UpdateCourseOrder([FromBody] UpdateCourseOrderDto dto)
    {
        var result = await _learningPathService.UpdateCourseOrderAsync(dto);
        if (!result) return NotFound();
        return Ok();
    }

    [HttpDelete("learning-paths/{learningPathId}/courses/{courseId}")]
    public async Task<IActionResult> RemoveCourseFromPath(int learningPathId, int courseId)
    {
        var result = await _learningPathService.RemoveCourseFromPathAsync(learningPathId, courseId);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpGet("courses/available")]
    public async Task<IActionResult> GetAvailableCourses()
    {
        var courses = await _learningPathService.GetAvailableCoursesAsync();
        return Ok(courses);
    }
}
