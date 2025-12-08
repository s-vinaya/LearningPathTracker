using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Application.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly FileLogger _logger;

    public ProfileController(IProfileService profileService, FileLogger logger)
    {
        _profileService = profileService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                if (string.IsNullOrEmpty(emailClaim))
                {
                    _logger.LogWarning("Controller", "GetProfile: User not authenticated");
                    return Unauthorized(new { message = "User ID not found in token" });
                }
                _logger.LogInfo("Controller", $"GetProfile by email: {emailClaim}");
                var profileByEmail = await _profileService.GetProfileByEmailAsync(emailClaim);
                return profileByEmail == null ? NotFound(new { message = "Profile not found" }) : Ok(profileByEmail);
            }

            _logger.LogInfo("Controller", $"GetProfile: userId={userId}");
            var profile = await _profileService.GetProfileAsync(userId);
            if (profile == null)
            {
                _logger.LogWarning("Controller", $"GetProfile: Profile not found - userId={userId}");
                return NotFound(new { message = "Profile not found" });
            }

            return Ok(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetProfile failed", ex);
            return StatusCode(500, new { message = "Error retrieving profile" });
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto, [FromServices] learning_path_tracker.Database.AppDbContext context)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            
            int userId;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out userId))
            {
                if (string.IsNullOrEmpty(emailClaim))
                {
                    _logger.LogWarning("Controller", "UpdateProfile: User not authenticated");
                    return Unauthorized(new { message = "User not authenticated" });
                }
                var userProfile = await _profileService.GetProfileByEmailAsync(emailClaim);
                if (userProfile == null)
                {
                    _logger.LogWarning("Controller", $"UpdateProfile: Profile not found - email={emailClaim}");
                    return NotFound(new { message = "Profile not found" });
                }
                userId = userProfile.Id;
            }

            _logger.LogInfo("Controller", $"UpdateProfile: userId={userId}");
            
            // Get user to check role and department changes
            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            
            var oldDepartment = user.Department;
            var isManager = user.Role == "Manager";
            
            var profile = await _profileService.UpdateProfileAsync(userId, dto);
            if (profile == null)
            {
                _logger.LogWarning("Controller", $"UpdateProfile: Profile not found - userId={userId}");
                return NotFound(new { message = "Profile not found" });
            }

            // If manager sets/changes department, auto-assign employees
            if (isManager && !string.IsNullOrEmpty(dto.Department) && dto.Department != oldDepartment)
            {
                var existingManager = await context.Users
                    .FirstOrDefaultAsync(u => u.Department == dto.Department && u.Role == "Manager" && u.Id != userId);
                
                if (existingManager != null)
                {
                    return BadRequest(new { message = $"Department '{dto.Department}' already has a manager: {existingManager.FullName}" });
                }
                
                // Assign all employees from this department to this manager
                var employeesToAssign = await context.Users
                    .Where(u => u.Role == "Employee" && u.Department == dto.Department)
                    .ToListAsync();
                
                foreach (var emp in employeesToAssign)
                {
                    emp.ManagerId = userId;
                }
                
                await context.SaveChangesAsync();
                _logger.LogInfo("Controller", $"Manager assigned to department: userId={userId}, department={dto.Department}, employeesAssigned={employeesToAssign.Count}");
            }

            _logger.LogInfo("Controller", $"Profile updated: userId={userId}");
            return Ok(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "UpdateProfile failed", ex);
            return StatusCode(500, new { message = "Error updating profile" });
        }
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadProfileImage(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Controller", "UploadProfileImage: No file uploaded");
                return BadRequest(new { message = "No file uploaded" });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            
            int userId;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out userId))
            {
                if (string.IsNullOrEmpty(emailClaim))
                {
                    _logger.LogWarning("Controller", "UploadProfileImage: User not authenticated");
                    return Unauthorized(new { message = "User not authenticated" });
                }
                var userProfile = await _profileService.GetProfileByEmailAsync(emailClaim);
                if (userProfile == null)
                {
                    _logger.LogWarning("Controller", $"UploadProfileImage: Profile not found - email={emailClaim}");
                    return NotFound(new { message = "Profile not found" });
                }
                userId = userProfile.Id;
            }

            _logger.LogInfo("Controller", $"UploadProfileImage: userId={userId}");
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var imageData = memoryStream.ToArray();

            var success = await _profileService.UploadProfileImageAsync(userId, imageData);
            if (!success)
            {
                _logger.LogWarning("Controller", $"UploadProfileImage: Failed - userId={userId}");
                return NotFound(new { message = "Profile not found" });
            }

            _logger.LogInfo("Controller", $"Profile image uploaded: userId={userId}");
            return Ok(new { message = "Profile image uploaded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "UploadProfileImage failed", ex);
            return StatusCode(500, new { message = "Error uploading profile image" });
        }
    }
}
