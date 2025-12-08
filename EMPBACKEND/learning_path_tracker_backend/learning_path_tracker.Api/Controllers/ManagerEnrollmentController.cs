using learning_path_tracker.Data.Repositories;
using learning_path_tracker.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/manager/enrollment")]
[Authorize(Roles = "Manager")]
public class ManagerEnrollmentController : ControllerBase
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILearningPathRepository _learningPathRepository;

    public ManagerEnrollmentController(
        IEnrollmentRepository enrollmentRepository,
        ILearningPathRepository learningPathRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _learningPathRepository = learningPathRepository;
    }

    [HttpPost("assign-learning-path")]
    public async Task<IActionResult> AssignLearningPath([FromBody] AssignLearningPathDto dto)
    {
        var learningPath = await _learningPathRepository.GetByIdAsync(dto.LearningPathId);
        if (learningPath == null)
            return NotFound("Learning path not found");

        foreach (var course in learningPath.LearningPathCourses)
        {
            var existingEnrollment = (await _enrollmentRepository.GetByUserIdAsync(dto.UserId))
                .FirstOrDefault(e => e.CourseId == course.CourseId);

            if (existingEnrollment == null)
            {
                await _enrollmentRepository.AddAsync(new Enrollment
                {
                    UserId = dto.UserId,
                    CourseId = course.CourseId,
                    EnrolledAt = DateTime.UtcNow,
                    Progress = 0
                });
            }
        }

        return Ok(new { message = "Learning path assigned successfully" });
    }

    [HttpPut("update-progress")]
    public async Task<IActionResult> UpdateProgress([FromBody] UpdateProgressDto dto)
    {
        var enrollments = await _enrollmentRepository.GetByUserIdAsync(dto.UserId);
        var enrollment = enrollments.FirstOrDefault(e => e.CourseId == dto.CourseId);
        
        if (enrollment == null)
            return NotFound("Enrollment not found");

        enrollment.Progress = dto.Progress;
        if (dto.Progress >= 100)
        {
            enrollment.CompletedAt = DateTime.UtcNow;
        }
        
        await _enrollmentRepository.UpdateAsync(enrollment);
        return Ok(new { message = "Progress updated successfully" });
    }
}

public class UpdateProgressDto
{
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public int Progress { get; set; }
}

public class AssignLearningPathDto
{
    public int UserId { get; set; }
    public int LearningPathId { get; set; }
}
