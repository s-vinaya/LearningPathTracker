using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMPBACKEND.Data;
using EMPBACKEND.DTOs;
using EMPBACKEND.Models;
using EMPBACKEND.Services;
using BCrypt.Net;
using System.Security.Claims;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var users = await _context.Users
                .Where(u => u.Id != currentUserId)
                .Include(u => u.Department)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role,
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department!.Name,
                    IsActive = u.IsActive,
                    IsApproved = u.IsApproved,
                    CreatedDate = u.CreatedDate
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                DepartmentId = user.DepartmentId,
                DepartmentName = user.Department?.Name,
                IsActive = user.IsActive,
                IsApproved = user.IsApproved,
                CreatedDate = user.CreatedDate
            };

            return Ok(userDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                DepartmentId = dto.DepartmentId,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                DepartmentId = user.DepartmentId,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
                return BadRequest("Email already exists");

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username && u.Id != id))
                return BadRequest("Username already exists");

            user.Username = dto.Username;
            user.Email = dto.Email;
            user.Role = dto.Role;
            user.DepartmentId = dto.DepartmentId;
            user.IsActive = dto.IsActive;
            user.IsApproved = dto.IsApproved;
            user.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("department/{departmentId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByDepartment(int departmentId)
        {
            var users = await _context.Users
                .Where(u => u.DepartmentId == departmentId)
                .Include(u => u.Department)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role,
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department!.Name,
                    IsActive = u.IsActive,
                    IsApproved = u.IsApproved,
                    CreatedDate = u.CreatedDate
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.IsApproved = true;
            user.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Send approval email
            var emailService = HttpContext.RequestServices.GetRequiredService<IEmailService>();
            await emailService.SendApprovalEmailAsync(user.Email, user.FirstName, user.LastName);

            return Ok("User approved successfully");
        }

        [HttpPut("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromBody] string newRole)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            if (newRole != "Admin" && newRole != "Manager" && newRole != "Employee")
                return BadRequest("Invalid role");

            user.Role = newRole;
            user.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok($"User role changed to {newRole} successfully");
        }

        [HttpGet("pending-approval")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetPendingApprovalUsers()
        {
            var users = await _context.Users
                .Where(u => !u.IsApproved && u.Role == "Employee")
                .Include(u => u.Department)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role,
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department!.Name,
                    IsActive = u.IsActive,
                    IsApproved = u.IsApproved,
                    CreatedDate = u.CreatedDate
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            // Send rejection email before deleting
            var emailService = HttpContext.RequestServices.GetRequiredService<IEmailService>();
            await emailService.SendRejectionEmailAsync(user.Email, user.FirstName, user.LastName);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User rejected and removed successfully");
        }
    }
}