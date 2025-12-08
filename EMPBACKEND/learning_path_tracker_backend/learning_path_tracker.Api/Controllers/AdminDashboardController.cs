using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Application.Logging;
using learning_path_tracker.Api.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize]
[AuthorizeRoles("Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly FileLogger _logger;

    public AdminDashboardController(IDashboardService dashboardService, FileLogger logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        try
        {
            _logger.LogInfo("Controller", "GetSummary called");
            var summary = await _dashboardService.GetDashboardSummaryAsync();
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetSummary failed", ex);
            return StatusCode(500, new { message = "Error retrieving dashboard summary" });
        }
    }

    [HttpGet("engagement")]
    public async Task<IActionResult> GetEngagement([FromQuery] int range = 7)
    {
        try
        {
            _logger.LogInfo("Controller", $"GetEngagement: range={range}");
            var engagement = await _dashboardService.GetEngagementDataAsync(range);
            return Ok(engagement);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetEngagement failed", ex);
            return StatusCode(500, new { message = "Error retrieving engagement data" });
        }
    }

    [HttpGet("recent-activities")]
    public async Task<IActionResult> GetRecentActivities()
    {
        try
        {
            _logger.LogInfo("Controller", "GetRecentActivities called");
            var activities = await _dashboardService.GetRecentActivitiesAsync();
            return Ok(activities);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetRecentActivities failed", ex);
            return StatusCode(500, new { message = "Error retrieving recent activities" });
        }
    }
}
