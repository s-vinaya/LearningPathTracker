using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Data.Repositories;
using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using learning_path_tracker.Application.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace learning_path_tracker.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly FileLogger _logger;
    private readonly IPlatformSettingsService _settingsService;
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, FileLogger logger, IPlatformSettingsService settingsService, AppDbContext context, EmailService emailService)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
        _settingsService = settingsService;
        _context = context;
        _emailService = emailService;
    }

    public async Task<UserResponseDTO?> RegisterAsync(RegisterRequestDTO dto)
    {
        try
        {
            _logger.LogInfo("Service", $"RegisterAsync: Checking if user exists - {dto.Email}");
            if (await _userRepository.GetByEmailAsync(dto.Email) != null)
            {
                _logger.LogWarning("Service", $"RegisterAsync: User already exists - {dto.Email}");
                return null;
            }

            var settings = await _settingsService.GetSettingsAsync();
            var salt = GenerateSalt();
            var user = new User
            {
                FullName = dto.FullName,
                Name = dto.FullName,
                Email = dto.Email,
                Salt = salt,
                PasswordHash = HashPassword(dto.Password, salt),
                PhoneNumber = dto.PhoneNumber,
                Department = dto.Department,
                JobTitle = dto.JobTitle,
                Role = "Employee",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsApproved = settings.AutoEnroll
            };

            // Auto-assign manager if department has one
            if (!string.IsNullOrEmpty(dto.Department))
            {
                var manager = await _context.Users
                    .FirstOrDefaultAsync(u => u.Department == dto.Department && u.Role == "Manager");
                if (manager != null)
                {
                    user.ManagerId = manager.Id;
                }
            }

            _logger.LogInfo("Service", $"RegisterAsync: Creating user - {dto.Email}");
            await _userRepository.CreateAsync(user);
            _logger.LogInfo("Service", $"RegisterAsync: User created successfully - {dto.Email}");
            
            return new UserResponseDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Department = user.Department,
                JobTitle = user.JobTitle,
                Role = user.Role,
                LastLogin = user.LastLogin ?? DateTime.MinValue
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Service", $"RegisterAsync failed for {dto.Email}", ex);
            throw;
        }
    }

    public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO dto)
    {
        try
        {
            _logger.LogInfo("Service", $"LoginAsync: Attempting login for {dto.Email}");
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("Service", $"LoginAsync: User not found - {dto.Email}");
                return null;
            }

            var hashedPassword = HashPassword(dto.Password, user.Salt);
            if (hashedPassword != user.PasswordHash)
            {
                _logger.LogWarning("Service", $"LoginAsync: Invalid password - {dto.Email}");
                return null;
            }

            if (!user.IsApproved)
            {
                _logger.LogWarning("Service", $"LoginAsync: User not approved - {dto.Email}");
                return null;
            }

            user.LastLogin = DateTime.UtcNow.AddHours(5).AddMinutes(30);
            await _userRepository.UpdateAsync(user);
            _logger.LogInfo("Service", $"LoginAsync: Login successful - {dto.Email}");
            
            return new LoginResponseDTO
            {
                Token = GenerateToken(user.Email, user.Role, user.Id, user.Department),
                User = new UserResponseDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Department = user.Department ?? string.Empty,
                    JobTitle = user.JobTitle ?? string.Empty,
                    Role = user.Role,
                    ProfileImageBase64 = user.ProfileImage != null ? Convert.ToBase64String(user.ProfileImage) : string.Empty,
                    LastLogin = user.LastLogin ?? DateTime.MinValue
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Service", $"LoginAsync failed for {dto.Email}", ex);
            throw;
        }
    }

    private string GenerateToken(string email, string role, int userId, string? department = null)
    {
        var claimsList = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("userId", userId.ToString()),
            new Claim("id", userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };
        
        if (!string.IsNullOrEmpty(department))
            claimsList.Add(new Claim("Department", department));
        
        var claims = claimsList.ToArray();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateSalt()
    {
        var saltBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    private string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var saltedPassword = password + salt;
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hashBytes);
        }
    }

    public async Task<bool> CheckEmailExistsAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email) != null;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return false;

        var token = Guid.NewGuid().ToString();
        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.PasswordResetTokens.Add(resetToken);
        await _context.SaveChangesAsync();

        var resetLink = $"http://localhost:4200/reset-password?token={token}";
        var emailBody = $@"
            <h2>Password Reset Request</h2>
            <p>Click the link below to reset your password:</p>
            <a href='{resetLink}'>Reset Password</a>
            <p>This link will expire in 1 hour.</p>
        ";

        await _emailService.SendEmailAsync(email, "Password Reset Request", emailBody);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        var resetToken = await _context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);

        if (resetToken == null) return false;

        var salt = GenerateSalt();
        resetToken.User.Salt = salt;
        resetToken.User.PasswordHash = HashPassword(newPassword, salt);
        resetToken.IsUsed = true;

        await _context.SaveChangesAsync();
        return true;
    }
}
