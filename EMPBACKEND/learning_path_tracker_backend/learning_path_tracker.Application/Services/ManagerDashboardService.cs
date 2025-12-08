using learning_path_tracker.Application.DTOs.Dashboard;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Database;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Application.Services;

public class ManagerDashboardService : IManagerDashboardService
{
    private readonly AppDbContext _context;

    public ManagerDashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ManagerDashboardDto> GetManagerDashboardAsync(int managerId)
    {
        var manager = await _context.Users.FindAsync(managerId);
        if (manager == null)
            throw new KeyNotFoundException("Manager not found");

        var managerDepartment = manager.Department;

        var totalEmployees = await _context.Users
            .Where(u => (u.Department == managerDepartment || u.ManagerId == managerId) && u.Role == "Employee" && u.IsActive)
            .CountAsync();

        var assignments = await _context.Assignments
            .Include(a => a.Employee)
            .Include(a => a.LearningPath)
            .Where(a => a.AssignedByManagerId == managerId)
            .ToListAsync();

        var activeLearningPaths = assignments
            .Where(a => a.Status != "Completed")
            .Select(a => a.LearningPathId)
            .Distinct()
            .Count();

        var completedLearningPaths = assignments
            .Count(a => a.Status == "Completed");

        var pendingApprovals = await _context.Approvals
            .Where(a => a.ManagerId == managerId && a.Status == "Pending")
            .CountAsync();

        var now = DateTime.UtcNow;
        var upcomingDeadlines = assignments
            .Where(a => a.DueDate.HasValue && 
                       a.DueDate.Value >= now && 
                       a.DueDate.Value <= now.AddDays(7) &&
                       a.Status != "Completed")
            .OrderBy(a => a.DueDate)
            .Select(a => new UpcomingDeadlineDto
            {
                AssignmentId = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FullName,
                LearningPathTitle = a.LearningPath.Title,
                DueDate = a.DueDate!.Value,
                ProgressPercent = a.ProgressPercent,
                Status = a.Status
            })
            .ToList();

        return new ManagerDashboardDto
        {
            TotalEmployees = totalEmployees,
            ActiveLearningPaths = activeLearningPaths,
            CompletedLearningPaths = completedLearningPaths,
            PendingApprovals = pendingApprovals,
            UpcomingDeadlines = upcomingDeadlines
        };
    }

    public async Task<List<UpcomingDeadlineDto>> GetUpcomingDeadlinesAsync(int managerId, int days)
    {
        var now = DateTime.UtcNow;
        var futureDate = now.AddDays(days);
        
        var assignments = await _context.Assignments
            .Include(a => a.Employee)
            .Include(a => a.LearningPath)
            .Where(a => a.AssignedByManagerId == managerId &&
                       a.DueDate.HasValue && 
                       a.DueDate.Value >= now && 
                       a.DueDate.Value <= futureDate &&
                       a.Status != "Completed")
            .OrderBy(a => a.DueDate)
            .Select(a => new UpcomingDeadlineDto
            {
                AssignmentId = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FullName,
                EmployeeEmail = a.Employee.Email,
                EmployeeDepartment = a.Employee.Department,
                LearningPathTitle = a.LearningPath.Title,
                DueDate = a.DueDate!.Value,
                ProgressPercent = a.ProgressPercent,
                Status = a.Status,
                AssignedDate = a.AssignedDate,
                DaysRemaining = (int)(a.DueDate.Value - now).TotalDays
            })
            .ToListAsync();

        return assignments;
    }
}
