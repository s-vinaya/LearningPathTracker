using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/manager/assignments")]
[Authorize]
public class ManagerAssignmentsController : ControllerBase
{
    private readonly IManagerAssignmentService _assignmentService;

    public ManagerAssignmentsController(IManagerAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    private int GetManagerId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userId, out int managerId) ? managerId : 0;
    }

    [HttpGet("learning-paths")]
    public async Task<IActionResult> GetLearningPaths()
    {
        var paths = await _assignmentService.GetLearningPathsAsync();
        return Ok(paths);
    }

    [HttpGet("employees")]
    public async Task<IActionResult> GetTeamEmployees()
    {
        var managerId = GetManagerId();
        if (managerId == 0) return Unauthorized();

        var employees = await _assignmentService.GetTeamEmployeesAsync(managerId);
        return Ok(employees);
    }

    [HttpGet("employees/debug")]
    public async Task<IActionResult> GetTeamEmployeesDebug()
    {
        var managerId = GetManagerId();
        return Ok(new { ManagerId = managerId });
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignLearningPath([FromBody] AssignLearningPathRequestDto dto)
    {
        var managerId = GetManagerId();
        if (managerId == 0) return Unauthorized();

        var result = await _assignmentService.AssignLearningPathAsync(managerId, dto);
        return result ? Ok(new { message = "Learning path assigned successfully" }) : BadRequest();
    }

    [HttpPost("bulk-assign")]
    public async Task<IActionResult> BulkAssignLearningPath([FromBody] BulkAssignDto dto)
    {
        var managerId = GetManagerId();
        if (managerId == 0) return Unauthorized();

        var result = await _assignmentService.BulkAssignLearningPathAsync(managerId, dto);
        return result ? Ok(new { message = "Learning path assigned to all team members" }) : BadRequest();
    }

    [HttpPut("reassign")]
    public async Task<IActionResult> ReassignLearningPath([FromBody] ReassignDto dto)
    {
        var managerId = GetManagerId();
        if (managerId == 0) return Unauthorized();

        var result = await _assignmentService.ReassignLearningPathAsync(managerId, dto);
        return result ? Ok(new { message = "Learning path reassigned successfully" }) : NotFound();
    }

    [HttpDelete("{assignmentId}")]
    public async Task<IActionResult> RemoveAssignment(int assignmentId)
    {
        var managerId = GetManagerId();
        if (managerId == 0) return Unauthorized();

        var result = await _assignmentService.RemoveAssignmentAsync(managerId, assignmentId);
        return result ? Ok(new { message = "Assignment removed successfully" }) : NotFound();
    }
}
