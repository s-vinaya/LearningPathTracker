using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeCourseRequestController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeeCourseRequestController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("request-custom")]
    public async Task<IActionResult> RequestCustomCourse([FromBody] CustomCourseRequestDto dto)
    {
        var employee = await _context.Users.FindAsync(dto.EmployeeId);
        if (employee == null) return NotFound("Employee not found");

        var manager = await _context.Users
            .FirstOrDefaultAsync(u => u.Department == employee.Department && u.Role == "Manager");
        
        if (manager == null) return BadRequest("No manager found for your department");

        var approval = new Approval
        {
            EmployeeId = dto.EmployeeId,
            ManagerId = manager.Id,
            Type = "CourseRequest",
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            RequestDetails = $"Course: {dto.CourseName}|Reason: {dto.Reason}"
        };

        _context.Approvals.Add(approval);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Course request submitted successfully", approvalId = approval.Id });
    }

    [HttpGet("my-requests/{employeeId}")]
    public async Task<IActionResult> GetMyRequests(int employeeId)
    {
        var requests = await _context.Approvals
            .Include(a => a.Manager)
            .Where(a => a.EmployeeId == employeeId && a.Type == "CourseRequest")
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.Type,
                a.Status,
                a.CreatedAt,
                a.ReviewedAt,
                ManagerName = a.Manager.FullName,
                a.ReviewerComments,
                RequestDetails = a.RequestDetails
            })
            .ToListAsync();

        return Ok(requests);
    }
}

public class CourseRequestDto
{
    public int EmployeeId { get; set; }
    public int CourseId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class CustomCourseRequestDto
{
    public int EmployeeId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
