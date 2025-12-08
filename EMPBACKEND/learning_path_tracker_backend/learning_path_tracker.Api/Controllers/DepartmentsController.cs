using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DepartmentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _context.Departments
            .Include(d => d.Manager)
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.Description,
                d.ManagerId,
                managerName = d.Manager != null ? d.Manager.Name : null,
                d.CreatedAt,
                d.IsActive,
                employeeCount = _context.Users.Count(u => u.Department == d.Name)
            })
            .ToListAsync();

        return Ok(departments);
    }

    [HttpGet("unassigned-employees")]
    public async Task<IActionResult> GetUnassignedEmployees()
    {
        var unassignedUsers = await _context.Users
            .Where(u => u.Role == "Employee" && (u.Department == null || u.Department == ""))
            .Select(u => new { u.Id, u.Name, u.Email, u.Role })
            .ToListAsync();

        return Ok(unassignedUsers);
    }

    [HttpGet("{id}/employees")]
    public async Task<IActionResult> GetDepartmentEmployees(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        var employees = await _context.Users
            .Where(u => u.Department == department.Name)
            .Select(u => new { u.Id, u.Name, u.Email, u.Role })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var department = await _context.Departments
            .Include(d => d.Manager)
            .Where(d => d.Id == id)
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.Description,
                d.ManagerId,
                managerName = d.Manager != null ? d.Manager.Name : null,
                d.CreatedAt,
                d.IsActive,
                employees = _context.Users
                    .Where(u => u.Department == d.Name)
                    .Select(u => new { u.Id, u.Name, u.Email, u.Role })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        return department == null ? NotFound() : Ok(department);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Department department)
    {
        department.CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30);
        department.IsActive = true;
        
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return Ok(department);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Department department)
    {
        var existing = await _context.Departments.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = department.Name;
        existing.Description = department.Description;
        existing.ManagerId = department.ManagerId;
        existing.IsActive = department.IsActive;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Department deleted successfully" });
    }

    [HttpPut("{id}/assign-manager")]
    public async Task<IActionResult> AssignManager(int id, [FromBody] int managerId)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        department.ManagerId = managerId;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Manager assigned successfully" });
    }

    [HttpPut("{id}/assign-employee")]
    public async Task<IActionResult> AssignEmployee(int id, [FromBody] AssignEmployeeRequest request)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null) return NotFound();

        user.Department = department.Name;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Employee assigned successfully" });
    }

    [HttpPut("{id}/remove-employee/{userId}")]
    public async Task<IActionResult> RemoveEmployee(int id, int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        user.Department = null;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Employee removed successfully" });
    }

    [HttpPut("change-employee-department")]
    public async Task<IActionResult> ChangeEmployeeDepartment([FromBody] ChangeEmployeeDepartmentRequest request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null) return NotFound();

        var department = await _context.Departments.FindAsync(request.NewDepartmentId);
        if (department == null) return NotFound();

        user.Department = department.Name;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Employee department changed successfully" });
    }

    // TODO: Move this to post deployment script 
    [HttpPost("seed")]
    public async Task<IActionResult> SeedDepartments()
    {
        var existingCount = await _context.Departments.CountAsync();
        if (existingCount > 0)
        {
            return Ok(new { message = "Departments already exist" });
        }

        var departments = new[]
        {
            new Department { Name = "Engineering", Description = "Software development and engineering", CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30), IsActive = true },
            new Department { Name = "Marketing", Description = "Marketing and brand management", CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30), IsActive = true },
            new Department { Name = "Sales", Description = "Sales and business development", CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30), IsActive = true },
            new Department { Name = "HR", Description = "Human resources and recruitment", CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30), IsActive = true },
            new Department { Name = "Operations", Description = "Operations and logistics", CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30), IsActive = true },
            new Department { Name = "Finance", Description = "Finance and accounting", CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30), IsActive = true }
        };

        _context.Departments.AddRange(departments);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Departments seeded successfully", count = departments.Length });
    }
}

public class AssignEmployeeRequest
{
    public int UserId { get; set; }
}

public class ChangeEmployeeDepartmentRequest
{
    public int UserId { get; set; }
    public int NewDepartmentId { get; set; }
}
