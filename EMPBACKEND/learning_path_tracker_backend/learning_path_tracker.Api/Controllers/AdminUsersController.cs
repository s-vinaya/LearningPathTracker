using learning_path_tracker.Application.DTOs.Users;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Application.Constants;
using learning_path_tracker.Data.Repositories;
using learning_path_tracker.Application.Logging;
using learning_path_tracker.Api.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize]
[AuthorizeRoles("Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly FileLogger _logger;

    public AdminUsersController(IUserService userService, IUserRepository userRepository, FileLogger logger)
    {
        _userService = userService;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            _logger.LogInfo("Controller", "GetAll users called");
            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var users = await _userService.GetAllUsersAsync();
            var filteredUsers = users.Where(u => u.Email != currentUserEmail).ToList();
            return Ok(filteredUsers);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetAll users failed", ex);
            return StatusCode(500, new { message = "Error retrieving users" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            _logger.LogInfo("Controller", $"GetById: userId={id}");
            var user = await _userRepository.GetByIdWithEnrollmentsAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Controller", $"GetById: User not found - userId={id}");
                return NotFound(new { message = EmailConstants.UserNotFoundMessage });
            }

            var userDetails = new UserDetailsDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                IsApproved = user.IsApproved,
                ProfileImageBase64 = user.ProfileImage != null ? Convert.ToBase64String(user.ProfileImage) : null,
                Enrollments = user.Enrollments.Select(e => new UserEnrollmentDto
                {
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    Progress = e.Progress,
                    EnrolledAt = e.EnrolledAt,
                    CompletedAt = e.CompletedAt
                }).ToList()
            };

            return Ok(userDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"GetById failed: userId={id}", ex);
            return StatusCode(500, new { message = "Error retrieving user" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("Controller", "Create: Invalid input data");
                return BadRequest(new { message = "Email and name are required" });
            }

            _logger.LogInfo("Controller", $"Create user: email={dto.Email}");
            var user = await _userService.CreateUserAsync(dto);
            _logger.LogInfo("Controller", $"User created: userId={user.Id}");
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "Create user failed", ex);
            return StatusCode(500, new { message = "Error creating user" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            _logger.LogInfo("Controller", $"Update user: userId={id}");
            var user = await _userService.UpdateUserAsync(id, dto);
            if (user == null)
            {
                _logger.LogWarning("Controller", $"Update: User not found - userId={id}");
                return NotFound(new { message = EmailConstants.UserNotFoundMessage });
            }
            _logger.LogInfo("Controller", $"User updated: userId={id}");
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"Update user failed: userId={id}", ex);
            return StatusCode(500, new { message = "Error updating user" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInfo("Controller", $"Delete user: userId={id}");
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                _logger.LogWarning("Controller", $"Delete: User not found - userId={id}");
                return NotFound(new { message = EmailConstants.UserNotFoundMessage });
            }
            _logger.LogInfo("Controller", $"User deleted: userId={id}");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"Delete user failed: userId={id}", ex);
            return StatusCode(500, new { message = "Error deleting user" });
        }
    }

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveUser(int id, [FromServices] learning_path_tracker.Application.Services.EmailService emailService)
    {
        try
        {
            _logger.LogInfo("Controller", $"ApproveUser: userId={id}");
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Controller", $"ApproveUser: User not found - userId={id}");
                return NotFound(new { message = EmailConstants.UserNotFoundMessage });
            }

            user.IsApproved = true;
            await _userRepository.UpdateAsync(user);
            _logger.LogInfo("Controller", $"User approved: userId={id}");
            
            try
            {
                await emailService.SendEmailAsync(
                    user.Email,
                    EmailConstants.ApprovalSubject,
                    EmailConstants.GetApprovalBody(user.Name)
                );
                _logger.LogInfo("Controller", $"Approval email sent to: {user.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Controller", $"Email sending failed for: {user.Email}", ex);
            }
            
            return Ok(new { message = EmailConstants.UserApprovedMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"ApproveUser failed: userId={id}", ex);
            return StatusCode(500, new { message = "Error approving user" });
        }
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDto dto, [FromServices] learning_path_tracker.Database.AppDbContext context)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Role))
            {
                _logger.LogWarning("Controller", "UpdateRole: Invalid role");
                return BadRequest(new { message = "Role is required" });
            }

            _logger.LogInfo("Controller", $"UpdateRole: userId={id}, role={dto.Role}");
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Controller", $"UpdateRole: User not found - userId={id}");
                return NotFound(new { message = EmailConstants.UserNotFoundMessage });
            }

            if (dto.Role == "Manager" && !string.IsNullOrEmpty(user.Department))
            {
                var existingManager = await context.Users
                    .FirstOrDefaultAsync(u => u.Department == user.Department && u.Role == "Manager" && u.Id != id);
                
                if (existingManager != null)
                {
                    return BadRequest(new { message = $"Department '{user.Department}' already has a manager: {existingManager.FullName}" });
                }
                
                // Only assign employees from the SAME department
                var employeesToReassign = await context.Users
                    .Where(u => u.Role == "Employee" && u.Department == user.Department)
                    .ToListAsync();
                
                foreach (var emp in employeesToReassign)
                {
                    emp.ManagerId = id;
                }
            }

            user.Role = dto.Role;
            await _userRepository.UpdateAsync(user);
            await context.SaveChangesAsync();
            _logger.LogInfo("Controller", $"Role updated: userId={id}, role={dto.Role}");
            
            var message = dto.Role == "Manager" && string.IsNullOrEmpty(user.Department) 
                ? "Role updated successfully. Please set your department in your profile." 
                : "Role updated successfully";
            
            return Ok(new { message });
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"UpdateRole failed: userId={id}", ex);
            return StatusCode(500, new { message = "Error updating role" });
        }
    }

    [HttpPut("{employeeId}/assign-manager/{managerId}")]
    public async Task<IActionResult> AssignManager(int employeeId, int managerId, [FromServices] learning_path_tracker.Database.AppDbContext context)
    {
        try
        {
            var employee = await _userRepository.GetByIdAsync(employeeId);
            var manager = await _userRepository.GetByIdAsync(managerId);
            
            if (employee == null || manager == null)
                return NotFound(new { message = "User not found" });
            
            if (manager.Role != "Manager")
                return BadRequest(new { message = "Target user is not a manager" });
            
            employee.ManagerId = managerId;
            await _userRepository.UpdateAsync(employee);
            await context.SaveChangesAsync();
            
            return Ok(new { message = "Manager assigned successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"AssignManager failed: employeeId={employeeId}, managerId={managerId}", ex);
            return StatusCode(500, new { message = "Error assigning manager" });
        }
    }
}
