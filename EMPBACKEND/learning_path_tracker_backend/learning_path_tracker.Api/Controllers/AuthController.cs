using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Application.Services;
using learning_path_tracker.Application.Constants;
using learning_path_tracker.Application.Logging;
using learning_path_tracker.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly EmailService _emailService;
    private readonly FileLogger _logger;
    private readonly AppDbContext _context;

    public AuthController(AuthService authService, EmailService emailService, FileLogger logger, AppDbContext context)
    {
        _authService = authService;
        _emailService = emailService;
        _logger = logger;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDTO dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                _logger.LogWarning("Controller", "Register: Invalid input data");
                return BadRequest(new { message = "Email and password are required" });
            }

            _logger.LogInfo("Controller", $"Register attempt for email: {dto.Email}");
            var result = await _authService.RegisterAsync(dto);
            if (result == null)
            {
                _logger.LogWarning("Controller", $"Register failed: User already exists - {dto.Email}");
                return BadRequest(new { message = EmailConstants.UserAlreadyExistsMessage });
            }
            
            try
            {
                await _emailService.SendEmailAsync(
                    dto.Email,
                    EmailConstants.RegistrationSubject,
                    EmailConstants.GetRegistrationBody(dto.FullName)
                );
                _logger.LogInfo("Controller", $"Registration email sent to: {dto.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Controller", $"Email sending failed for: {dto.Email}", ex);
            }
            
            _logger.LogInfo("Controller", $"User registered successfully: {dto.Email}");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "Register: Unhandled exception", ex);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDTO dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                _logger.LogWarning("Controller", "Login: Invalid input data");
                return BadRequest(new { message = "Email and password are required" });
            }

            _logger.LogInfo("Controller", $"Login attempt for email: {dto.Email}");
            var result = await _authService.LoginAsync(dto);
            
            if (result == null)
            {
                _logger.LogWarning("Controller", $"Login failed: Invalid credentials - {dto.Email}");
                return Unauthorized(new { message = EmailConstants.InvalidCredentialsMessage, userNotFound = true });
            }

            _logger.LogInfo("Controller", $"User logged in successfully: {dto.Email}");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "Login: Unhandled exception", ex);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    [HttpPost("check-email")]
    public async Task<IActionResult> CheckEmail([FromBody] CheckEmailDto dto)
    {
        var exists = await _authService.CheckEmailExistsAsync(dto.Email);
        return Ok(new { exists });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var result = await _authService.ForgotPasswordAsync(dto.Email);
        if (!result)
            return NotFound(new { message = "Email not found" });
        
        return Ok(new { message = "Password reset link sent to your email" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var result = await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);
        if (!result)
            return BadRequest(new { message = "Invalid or expired token" });
        
        return Ok(new { message = "Password reset successfully" });
    }

    [HttpGet("departments")]
    public async Task<IActionResult> GetDepartments()
    {
        var departments = await _context.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .Select(d => new { d.Id, d.Name })
            .ToListAsync();
        
        return Ok(departments);
    }
}
