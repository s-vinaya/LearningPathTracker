using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMPBACKEND.Data;
using EMPBACKEND.DTOs;
using EMPBACKEND.Models;
using EMPBACKEND.Interfaces.Services;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public UserController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var users = await _context.Users
                .Where(u => u.Id != currentUserId)
                .Include(u => u.Department)
                .ToListAsync();

            var userDtos = _mapper.Map<List<UserDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UserDto>(user);

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

            var user = _mapper.Map<User>(dto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);

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

            _mapper.Map(dto, user);

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
                .ToListAsync();

            var userDtos = _mapper.Map<List<UserDto>>(users);
            return Ok(userDtos);
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
                .ToListAsync();

            var userDtos = _mapper.Map<List<UserDto>>(users);
            return Ok(userDtos);
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