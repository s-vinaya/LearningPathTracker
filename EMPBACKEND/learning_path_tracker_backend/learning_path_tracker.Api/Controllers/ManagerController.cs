using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Application.DTOs.Common;
using learning_path_tracker.Application.Logging;
using learning_path_tracker.Api.Attributes;
using learning_path_tracker.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/manager")]
[Authorize]
[AuthorizeRoles("Manager", "Admin")]
public class ManagerController : ControllerBase
{
    private readonly IManagerService _managerService;
    private readonly FileLogger _logger;

    public ManagerController(IManagerService managerService, FileLogger logger)
    {
        _managerService = managerService;
        _logger = logger;
    }

    private string GetManagerDepartment()
    {
        return User.FindFirst("Department")?.Value ?? string.Empty;
    }

    [HttpGet("dashboard-stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
                return Unauthorized();
            
            _logger.LogInfo("Controller", $"GetDashboardStats: managerId={managerId}");
            var stats = await _managerService.GetDashboardStatsAsync(managerId);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetDashboardStats failed", ex);
            return StatusCode(500, new { message = "Error retrieving dashboard stats" });
        }
    }

    [HttpGet("team-members")]
    public async Task<IActionResult> GetTeamMembers()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
                return Unauthorized();
            
            _logger.LogInfo("Controller", $"GetTeamMembers: managerId={managerId}");
            var members = await _managerService.GetTeamMembersAsync(managerId);
            return Ok(members);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetTeamMembers failed", ex);
            return StatusCode(500, new { message = "Error retrieving team members" });
        }
    }

    [HttpGet("learning-paths")]
    public async Task<IActionResult> GetLearningPaths()
    {
        try
        {
            _logger.LogInfo("Controller", "GetLearningPaths called");
            var paths = await _managerService.GetLearningPathsAsync();
            return Ok(paths);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetLearningPaths failed", ex);
            return StatusCode(500, new { message = "Error retrieving learning paths" });
        }
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GetReports()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
            return Unauthorized();
        
        var reports = await _managerService.GetReportsAsync(managerId);
        return Ok(reports);
    }

    [HttpGet("weekly-hours")]
    public async Task<IActionResult> GetWeeklyHours()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
            return Unauthorized();
        
        var hours = await _managerService.GetWeeklyHoursAsync(managerId);
        return Ok(hours);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
            return Unauthorized("User ID not found in token");

        var profile = await _managerService.GetManagerProfileAsync(managerId);
        return Ok(profile);
    }

    [HttpGet("assignments")]
    public async Task<IActionResult> GetAssignments()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
            return Unauthorized();

        var assignments = await _managerService.GetAssignmentsAsync(managerId);
        return Ok(assignments);
    }

    [HttpGet("approvals")]
    public async Task<IActionResult> GetPendingApprovals()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
            return Unauthorized();

        var approvals = await _managerService.GetPendingApprovalsAsync(managerId);
        return Ok(approvals);
    }



    [HttpPut("approvals/{id}/approve")]
    public async Task<IActionResult> ApproveRequest(int id, [FromBody] ApprovalActionDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
            return Unauthorized();

        var result = await _managerService.ApproveRequestAsync(id, managerId, dto.Comments);
        return result ? Ok(new { message = "Request approved" }) : NotFound();
    }

    [HttpPut("approvals/{id}/reject")]
    public async Task<IActionResult> RejectRequest(int id, [FromBody] ApprovalActionDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
            return Unauthorized();

        var result = await _managerService.RejectRequestAsync(id, managerId, dto.Comments);
        return result ? Ok(new { message = "Request rejected" }) : NotFound();
    }

    [HttpGet("employee-details/{employeeId}")]
    public async Task<IActionResult> GetEmployeeDetails(int employeeId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
            return Unauthorized();
        
        var details = await _managerService.GetEmployeeDetailsAsync(employeeId, managerId);
        return details != null ? Ok(details) : NotFound();
    }

    [HttpGet("employee-progress/{employeeId}")]
    public async Task<IActionResult> GetEmployeeProgress(int employeeId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
            return Unauthorized();
        
        var progress = await _managerService.GetEmployeeProgressAsync(employeeId, managerId);
        return progress != null ? Ok(progress) : NotFound();
    }

    [HttpPost("send-reminder/{employeeId}")]
    public async Task<IActionResult> SendReminder(int employeeId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
            return Unauthorized();
        
        var result = await _managerService.SendReminderAsync(employeeId, managerId);
        return result ? Ok(new { message = "Reminder sent successfully" }) : NotFound();
    }

    [HttpGet("{managerId}/profile")]
    [AllowAnonymous]
    public async Task<IActionResult> GetManagerProfile(int managerId)
    {
        var profile = await _managerService.GetManagerProfileAsync(managerId);
        return Ok(profile);
    }

    [HttpPut("{managerId}/profile")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateProfile(int managerId, [FromBody] UpdateProfileDto dto)
    {
        var result = await _managerService.UpdateManagerProfileAsync(managerId, dto);
        return result ? Ok(new { message = "Profile updated successfully" }) : NotFound();
    }

    [HttpGet("{managerId}/notifications")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNotifications(int managerId)
    {
        var settings = await _managerService.GetNotificationSettingsAsync(managerId);
        return Ok(settings);
    }

    [HttpPut("{managerId}/notifications")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateNotifications(int managerId, [FromBody] NotificationSettingsDto settings)
    {
        var result = await _managerService.UpdateNotificationSettingsAsync(managerId, settings);
        return result ? Ok(new { message = "Notification preferences saved" }) : BadRequest();
    }

    [HttpGet("{managerId}/team-settings")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTeamSettings(int managerId)
    {
        var settings = await _managerService.GetTeamSettingsAsync(managerId);
        return Ok(settings);
    }

    [HttpPut("{managerId}/team-settings")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateTeamSettings(int managerId, [FromBody] TeamSettingsDto settings)
    {
        var result = await _managerService.UpdateTeamSettingsAsync(managerId, settings);
        return result ? Ok(new { message = "Team settings updated" }) : BadRequest();
    }

    [HttpPost("{managerId}/profile/image")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadProfileImage(int managerId, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Controller", "UploadProfileImage: No file uploaded");
                return BadRequest(new { message = "No file uploaded" });
            }

            _logger.LogInfo("Controller", $"UploadProfileImage: managerId={managerId}");
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            var result = await _managerService.UpdateProfileImageAsync(managerId, imageBytes);
            if (!result)
            {
                _logger.LogWarning("Controller", $"UploadProfileImage: Manager not found - managerId={managerId}");
                return NotFound(new { message = "Manager not found" });
            }
            _logger.LogInfo("Controller", $"Profile image updated: managerId={managerId}");
            return Ok(new { message = "Profile image updated" });
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"UploadProfileImage failed: managerId={managerId}", ex);
            return StatusCode(500, new { message = "Error uploading profile image" });
        }
    }



    [HttpPost("{managerId}/approvals/decision")]
    [AllowAnonymous]
    public async Task<IActionResult> ApprovalDecision(int managerId, [FromBody] System.Text.Json.JsonElement body)
    {
        try
        {
            var approvalId = body.TryGetProperty("approvalId", out var appId) ? appId.GetInt32() : 0;
            var status = body.TryGetProperty("status", out var stat) ? stat.GetString() : "";
            var comments = body.TryGetProperty("reviewerComments", out var comm) ? comm.GetString() : "";
            
            _logger.LogInfo("Controller", $"ApprovalDecision: managerId={managerId}, approvalId={approvalId}, status={status}");
            
            if (approvalId == 0)
                return BadRequest(new { message = "Approval ID is required" });
            
            if (status == "Approved")
            {
                var result = await _managerService.ApproveRequestAsync(approvalId, managerId, comments);
                return Ok(new { message = "Request approved", success = true });
            }
            
            if (status == "Rejected")
            {
                if (string.IsNullOrWhiteSpace(comments))
                    return BadRequest(new { message = "Rejection reason is required" });
                    
                var result = await _managerService.RejectRequestAsync(approvalId, managerId, comments);
                return Ok(new { message = "Request rejected", success = true });
            }
            
            return BadRequest(new { message = "Invalid status" });
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"ApprovalDecision failed: {ex.Message}", ex);
            return StatusCode(500, new { message = ex.Message, success = false });
        }
    }

    [HttpPost("certificate-requests/{requestId}/approve")]
    public async Task<IActionResult> ApproveCertificateRequest(int requestId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
                return Unauthorized();

            _logger.LogInfo("Controller", $"ApproveCertificateRequest: managerId={managerId}, requestId={requestId}");
            var result = await _managerService.ApproveCertificateRequestAsync(requestId, managerId);
            _logger.LogInfo("Controller", $"Certificate request approved: requestId={requestId}");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"ApproveCertificateRequest failed: requestId={requestId}", ex);
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("certificate-requests/{requestId}/reject")]
    public async Task<IActionResult> RejectCertificateRequest(int requestId, [FromBody] ApproveCertificateRequestDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int managerId))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                return BadRequest(new { message = "Rejection reason is required" });

            _logger.LogInfo("Controller", $"RejectCertificateRequest: managerId={managerId}, requestId={requestId}");
            var result = await _managerService.RejectCertificateRequestAsync(requestId, managerId, dto.RejectionReason);
            _logger.LogInfo("Controller", $"Certificate request rejected: requestId={requestId}");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"RejectCertificateRequest failed: requestId={requestId}", ex);
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
