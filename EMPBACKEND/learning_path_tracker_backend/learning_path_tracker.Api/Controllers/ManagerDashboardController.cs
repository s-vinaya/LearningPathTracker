using learning_path_tracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/manager/{managerId}/dashboard")]
[Authorize(Roles = "Manager")]
public class ManagerDashboardController : ControllerBase
{
    private readonly IManagerDashboardService _dashboardService;

    public ManagerDashboardController(IManagerDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard(int managerId)
    {
        try
        {
            var dashboard = await _dashboardService.GetManagerDashboardAsync(managerId);
            return Ok(dashboard);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", details = ex.Message });
        }
    }

    [HttpGet("deadlines")]
    public async Task<IActionResult> GetUpcomingDeadlines(int managerId, [FromQuery] int? days = 7)
    {
        try
        {
            var deadlines = await _dashboardService.GetUpcomingDeadlinesAsync(managerId, days ?? 7);
            return Ok(deadlines);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving deadlines", details = ex.Message });
        }
    }
}

[ApiController]
[Route("api/manager/dashboard")]
[Authorize(Roles = "Manager")]
public class ManagerDashboardSimpleController : ControllerBase
{
    private readonly IManagerDashboardService _dashboardService;

    public ManagerDashboardSimpleController(IManagerDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("deadlines")]
    public async Task<IActionResult> GetUpcomingDeadlines([FromQuery] int? days = 7)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var managerId))
                return Unauthorized(new { message = "Manager not authenticated" });

            var deadlines = await _dashboardService.GetUpcomingDeadlinesAsync(managerId, days ?? 7);
            return Ok(deadlines);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving deadlines", details = ex.Message });
        }
    }
}
