using learning_path_tracker.Application.DTOs.Courses;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Application.Logging;
using learning_path_tracker.Api.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace learning_path_tracker.Api.Controllers;


[ApiController]
[Route("api/admin/courses")]
[Authorize]
[AuthorizeRoles("Admin")]
public class AdminCoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly FileLogger _logger;

    public AdminCoursesController(ICourseService courseService, FileLogger logger)
    {
        _courseService = courseService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            _logger.LogInfo("Controller", "GetAll courses called");
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetAll courses failed", ex);
            return StatusCode(500, new { message = "Error retrieving courses" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                _logger.LogWarning("Controller", "Create course: Invalid input");
                return BadRequest(new { message = "Title is required" });
            }

            _logger.LogInfo("Controller", $"Create course: {dto.Title}");
            var course = await _courseService.CreateCourseAsync(dto);
            _logger.LogInfo("Controller", $"Course created: courseId={course.Id}");
            return CreatedAtAction(nameof(GetAll), new { id = course.Id }, course);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "Create course failed", ex);
            return StatusCode(500, new { message = "Error creating course" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseDto dto)
    {
        try
        {
            _logger.LogInfo("Controller", $"Update course: courseId={id}");
            var course = await _courseService.UpdateCourseAsync(id, dto);
            if (course == null)
            {
                _logger.LogWarning("Controller", $"Update: Course not found - courseId={id}");
                return NotFound(new { message = "Course not found" });
            }
            _logger.LogInfo("Controller", $"Course updated: courseId={id}");
            return Ok(course);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"Update course failed: courseId={id}", ex);
            return StatusCode(500, new { message = "Error updating course" });
        }
    }

    [HttpGet("{id}/delete-impact")]
    public async Task<IActionResult> CheckDeleteImpact(int id)
    {
        try
        {
            var impact = await _courseService.CheckCourseDeleteImpactAsync(id);
            return Ok(impact);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"CheckDeleteImpact failed: courseId={id}", ex);
            return StatusCode(500, new { message = "Error checking delete impact" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInfo("Controller", $"Delete course: courseId={id}");
            var result = await _courseService.DeleteCourseAsync(id);
            if (!result)
            {
                _logger.LogWarning("Controller", $"Delete: Course not found - courseId={id}");
                return NotFound(new { message = "Course not found" });
            }
            _logger.LogInfo("Controller", $"Course deleted: courseId={id}");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"Delete course failed: courseId={id}", ex);
            return StatusCode(500, new { message = "Error deleting course" });
        }
    }

    [HttpGet("{id}/statistics")]
    public async Task<IActionResult> GetStatistics(int id)
    {
        var stats = await _courseService.GetCourseStatisticsAsync(id);
        if (stats == null) return NotFound();
        return Ok(stats);
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetCourseStatistics()
    {
        var stats = await _courseService.GetAllCourseStatisticsAsync();
        return Ok(stats);
    }

    [HttpPost("bulk-upload")]
    public async Task<IActionResult> BulkUpload([FromBody] BulkUploadRequest request)
    {
        try
        {
            if (request.YouTubeUrls == null || !request.YouTubeUrls.Any())
            {
                _logger.LogWarning("Controller", "BulkUpload: No URLs provided");
                return BadRequest(new { message = "YouTube URLs are required" });
            }

            _logger.LogInfo("Controller", $"BulkUpload: {request.YouTubeUrls.Count} URLs");
            var result = await _courseService.BulkUploadCoursesAsync(request.YouTubeUrls);
            _logger.LogInfo("Controller", "BulkUpload completed");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "BulkUpload failed", ex);
            return StatusCode(500, new { message = "Error uploading courses" });
        }
    }
}
