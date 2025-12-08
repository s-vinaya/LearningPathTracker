using learning_path_tracker.Data.Repositories;
using learning_path_tracker.Domain.Entities;
using learning_path_tracker.Application.DTOs.Common;
using learning_path_tracker.Application.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly FileLogger _logger;

    public EnrollmentController(IEnrollmentRepository enrollmentRepository, FileLogger logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> EnrollUser([FromBody] EnrollUserDto dto)
    {
        try
        {
            if (dto.UserId <= 0 || dto.CourseId <= 0)
            {
                _logger.LogWarning("Controller", "EnrollUser: Invalid input");
                return BadRequest(new { message = "Valid UserId and CourseId are required" });
            }

            _logger.LogInfo("Controller", $"EnrollUser: userId={dto.UserId}, courseId={dto.CourseId}");
            var enrollment = new Enrollment
            {
                UserId = dto.UserId,
                CourseId = dto.CourseId,
                EnrolledAt = DateTime.UtcNow,
                Progress = 0
            };

            await _enrollmentRepository.AddAsync(enrollment);
            _logger.LogInfo("Controller", $"User enrolled: userId={dto.UserId}, courseId={dto.CourseId}");
            return Ok(new { message = "User enrolled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"EnrollUser failed: userId={dto.UserId}, courseId={dto.CourseId}", ex);
            return StatusCode(500, new { message = "Error enrolling user" });
        }
    }
}
