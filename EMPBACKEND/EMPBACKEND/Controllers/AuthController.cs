using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMPBACKEND.Data;
using EMPBACKEND.DTOs;
using EMPBACKEND.Models;
using EMPBACKEND.Interfaces.Services;
using AutoMapper;
using BCrypt.Net;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IMapper _mapper;

        public AuthController(ApplicationDbContext context, IJwtService jwtService, IEmailService emailService, IPasswordHashingService passwordHashingService, IMapper mapper)
        {
            _context = context;
            _jwtService = jwtService;
            _emailService = emailService;
            _passwordHashingService = passwordHashingService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            // Username will be email, so we only need to check email uniqueness

            var salt = _passwordHashingService.GenerateSalt();
            var hashedPassword = _passwordHashingService.HashPassword(dto.Password, salt);

            var user = new User
            {
                Username = dto.Email, // Use email as username
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = hashedPassword,
                Salt = salt,
                Role = "Employee",
                DepartmentId = dto.DepartmentId,
                IsActive = true,
                IsApproved = false // New employees need admin approval
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Send welcome email
            await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName, user.LastName);

            var token = _jwtService.GenerateToken(user);
            var userDto = _mapper.Map<UserDto>(user);

            return Ok(new AuthResponseDto { Token = token, User = userDto });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            bool isValidPassword;
            if (string.IsNullOrEmpty(user?.Salt))
            {
                // Legacy BCrypt verification for existing users
                isValidPassword = user != null && BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
            }
            else
            {
                // New salt-based verification
                isValidPassword = _passwordHashingService.VerifyPassword(dto.Password, user.Salt, user.Password);
            }

            if (user == null || !isValidPassword)
                return BadRequest("Invalid credentials");

            if (!user.IsActive)
                return BadRequest("Account is deactivated");

            if (!user.IsApproved && user.Role == "Employee")
                return BadRequest("Your account is pending admin approval. Please contact your administrator.");

            var token = _jwtService.GenerateToken(user);
            var userDto = _mapper.Map<UserDto>(user);

            return Ok(new AuthResponseDto { Token = token, User = userDto });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return BadRequest("Email not found. Please register first.");

            var otpCode = new Random().Next(100000, 999999).ToString();
            var otp = new OtpCode
            {
                Email = dto.Email,
                Code = otpCode,
                ExpiryTime = DateTime.UtcNow.AddMinutes(10)
            };

            _context.OtpCodes.Add(otp);
            await _context.SaveChangesAsync();

            await _emailService.SendOtpEmailAsync(dto.Email, otpCode);

            return Ok("OTP sent to your email");
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            var otp = await _context.OtpCodes
                .FirstOrDefaultAsync(o => o.Email == dto.Email && o.Code == dto.OtpCode && !o.IsUsed);

            if (otp == null || otp.ExpiryTime < DateTime.UtcNow)
                return BadRequest("Invalid or expired OTP");

            return Ok("OTP verified successfully");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var otp = await _context.OtpCodes
                .FirstOrDefaultAsync(o => o.Email == dto.Email && o.Code == dto.OtpCode && !o.IsUsed);

            if (otp == null || otp.ExpiryTime < DateTime.UtcNow)
                return BadRequest("Invalid or expired OTP");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return BadRequest("User not found");

            var newSalt = _passwordHashingService.GenerateSalt();
            user.Password = _passwordHashingService.HashPassword(dto.NewPassword, newSalt);
            user.Salt = newSalt;
            user.UpdatedDate = DateTime.UtcNow;

            otp.IsUsed = true;

            await _context.SaveChangesAsync();
            await _emailService.SendPasswordResetConfirmationAsync(dto.Email);

            return Ok("Password reset successfully");
        }
    }
}