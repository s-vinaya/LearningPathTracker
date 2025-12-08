using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Application.Logging;
using learning_path_tracker.Api.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/manager/{managerId}")]
[Authorize]
public class ManagerApprovalsController : ControllerBase
{
    private readonly IManagerService _managerService;
    private readonly FileLogger _logger;

    public ManagerApprovalsController(IManagerService managerService, FileLogger logger)
    {
        _managerService = managerService;
        _logger = logger;
    }

    [HttpGet("approvals")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllApprovals(int managerId)
    {
        try
        {
            _logger.LogInfo("Controller", $"GetAllApprovals: managerId={managerId}");
            var result = await _managerService.GetAllApprovalsAsync(managerId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetAllApprovals failed", ex);
            return StatusCode(500, new { message = "Error retrieving approvals" });
        }
    }

    [HttpGet("approvals/pending")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPendingApprovals(int managerId)
    {
        try
        {
            _logger.LogInfo("Controller", $"GetPendingApprovals: managerId={managerId}");
            var result = await _managerService.GetPendingApprovalsAsync(managerId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetPendingApprovals failed", ex);
            return StatusCode(500, new { message = "Error retrieving approvals" });
        }
    }
}
