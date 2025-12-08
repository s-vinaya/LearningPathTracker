using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Data.Repositories;
using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Application.Services;

public class ManagerAssignmentService : IManagerAssignmentService
{
    private readonly AppDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly ILearningPathRepository _learningPathRepository;
    private readonly EmailService _emailService;

    public ManagerAssignmentService(
        AppDbContext context,
        IUserRepository userRepository,
        ILearningPathRepository learningPathRepository,
        EmailService emailService)
    {
        _context = context;
        _userRepository = userRepository;
        _learningPathRepository = learningPathRepository;
        _emailService = emailService;
    }

    public async Task<object> GetLearningPathsAsync()
    {
        var learningPaths = await _context.LearningPaths
            .Include(lp => lp.LearningPathCourses)
            .ThenInclude(lpc => lpc.Course)
            .Where(lp => lp.IsActive)
            .ToListAsync();

        var result = new List<object>();
        foreach (var lp in learningPaths)
        {
            var courses = await _context.LearningPathCourses
                .Include(lpc => lpc.Course)
                .Where(lpc => lpc.LearningPathId == lp.Id)
                .OrderBy(lpc => lpc.Order)
                .ToListAsync();

            var totalDuration = 0.0;
            var courseList = new List<object>();

            foreach (var lpc in courses)
            {
                var duration = 0;
                if (!string.IsNullOrEmpty(lpc.Course.VideoDuration))
                {
                    var parts = lpc.Course.VideoDuration.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int minutes))
                        duration = minutes;
                }
                else
                {
                    duration = lpc.Course.DurationHours * 60;
                }

                totalDuration += duration;

                courseList.Add(new
                {
                    Title = lpc.Course.Title,
                    Description = lpc.Course.Description,
                    Duration = lpc.Course.VideoDuration ?? $"{lpc.Course.DurationHours}:00"
                });
            }

            result.Add(new
            {
                Id = lp.Id,
                Title = lp.Title,
                Description = lp.Description,
                EstimatedDuration = Math.Round(totalDuration / 60.0, 1),
                DifficultyLevel = lp.DifficultyLevel,
                CourseCount = courses.Count,
                Courses = courseList
            });
        }

        return result;
    }

    public async Task<object> GetTeamEmployeesAsync(int managerId)
    {
        var manager = await _userRepository.GetByIdAsync(managerId);
        if (manager == null) return new List<object>();

        var employees = await _userRepository.GetAllAsync();
        var teamMembers = employees
            .Where(u => u.Role?.ToLower() == "employee" && u.IsActive && u.ManagerId == managerId)
            .ToList();

        var result = new List<object>();
        foreach (var employee in teamMembers)
        {
            var assignments = await _context.Assignments
                .Include(a => a.LearningPath)
                .Where(a => a.EmployeeId == employee.Id && a.Status != "Completed")
                .ToListAsync();

            var enrollments = await _context.Enrollments
                .Where(e => e.UserId == employee.Id)
                .Include(e => e.Course)
                .ToListAsync();

            var totalModules = enrollments.Count;
            var completedModules = enrollments.Count(e => e.Progress >= 80 && (!e.Course.QuizId.HasValue || e.QuizPassed));
            var overallProgress = totalModules > 0 ? (completedModules * 100 / totalModules) : 0;

            result.Add(new
            {
                Id = employee.Id,
                Name = employee.FullName,
                Email = employee.Email,
                Role = employee.JobTitle ?? "Employee",
                Department = employee.Department,
                Status = employee.IsActive ? "Active" : "Inactive",
                Progress = overallProgress,
                AssignedPaths = assignments.Select(a => new
                {
                    AssignmentId = a.Id,
                    LearningPathId = a.LearningPathId,
                    LearningPathTitle = a.LearningPath.Title,
                    DueDate = a.DueDate,
                    Status = a.Status,
                    Progress = a.ProgressPercent
                }).ToList()
            });
        }

        return result;
    }

    public async Task<bool> AssignLearningPathAsync(int managerId, AssignLearningPathRequestDto dto)
    {
        var learningPath = await _context.LearningPaths
            .Include(lp => lp.LearningPathCourses)
            .ThenInclude(lpc => lpc.Course)
            .FirstOrDefaultAsync(lp => lp.Id == dto.LearningPathId);
        if (learningPath == null) return false;

        var manager = await _userRepository.GetByIdAsync(managerId);
        if (manager == null) return false;

        foreach (var employeeId in dto.EmployeeIds)
        {
            var employee = await _userRepository.GetByIdAsync(employeeId);
            if (employee == null) continue;
            if (employee.ManagerId != managerId) continue;

            var existingAssignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && 
                                         a.LearningPathId == dto.LearningPathId && 
                                         a.Status != "Completed");

            if (existingAssignment != null) continue;

            var assignment = new Assignment
            {
                EmployeeId = employeeId,
                LearningPathId = dto.LearningPathId,
                AssignedByManagerId = managerId,
                AssignedDate = DateTime.UtcNow,
                DueDate = dto.DueDate,
                ManagerNotes = dto.ManagerNotes,
                Status = "Assigned",
                ProgressPercent = 0
            };

            _context.Assignments.Add(assignment);
            
            // Auto-enroll in all courses in the learning path
            foreach (var lpc in learningPath.LearningPathCourses)
            {
                var existingEnrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.UserId == employeeId && e.CourseId == lpc.CourseId);
                
                if (existingEnrollment == null)
                {
                    var enrollment = new Enrollment
                    {
                        UserId = employeeId,
                        CourseId = lpc.CourseId,
                        EnrolledAt = DateTime.UtcNow,
                        Progress = 0,
                        QuizPassed = false
                    };
                    _context.Enrollments.Add(enrollment);
                }
            }
            
            await SendAssignmentEmailAsync(employee, learningPath, dto.DueDate, dto.ManagerNotes);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BulkAssignLearningPathAsync(int managerId, BulkAssignDto dto)
    {
        var manager = await _userRepository.GetByIdAsync(managerId);
        if (manager == null) return false;

        var learningPath = await _context.LearningPaths
            .Include(lp => lp.LearningPathCourses)
            .ThenInclude(lpc => lpc.Course)
            .FirstOrDefaultAsync(lp => lp.Id == dto.LearningPathId);
        if (learningPath == null) return false;

        var employees = await _userRepository.GetAllAsync();
        var teamMembers = employees
            .Where(u => u.Role?.ToLower() == "employee" && u.ManagerId == managerId && u.IsActive)
            .ToList();

        foreach (var employee in teamMembers)
        {
            var existingAssignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id && 
                                         a.LearningPathId == dto.LearningPathId && 
                                         a.Status != "Completed");

            if (existingAssignment != null) continue;

            var assignment = new Assignment
            {
                EmployeeId = employee.Id,
                LearningPathId = dto.LearningPathId,
                AssignedByManagerId = managerId,
                AssignedDate = DateTime.UtcNow,
                DueDate = dto.DueDate,
                ManagerNotes = dto.ManagerNotes,
                Status = "Assigned",
                ProgressPercent = 0
            };

            _context.Assignments.Add(assignment);
            
            // Auto-enroll in all courses in the learning path
            foreach (var lpc in learningPath.LearningPathCourses)
            {
                var existingEnrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.UserId == employee.Id && e.CourseId == lpc.CourseId);
                
                if (existingEnrollment == null)
                {
                    var enrollment = new Enrollment
                    {
                        UserId = employee.Id,
                        CourseId = lpc.CourseId,
                        EnrolledAt = DateTime.UtcNow,
                        Progress = 0,
                        QuizPassed = false
                    };
                    _context.Enrollments.Add(enrollment);
                }
            }
            
            await SendAssignmentEmailAsync(employee, learningPath, dto.DueDate, dto.ManagerNotes);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReassignLearningPathAsync(int managerId, ReassignDto dto)
    {
        var assignment = await _context.Assignments
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.Id == dto.AssignmentId);

        if (assignment == null) return false;

        if (assignment.Employee.ManagerId != managerId) return false;

        var newLearningPath = await _context.LearningPaths.FindAsync(dto.NewLearningPathId);
        if (newLearningPath == null) return false;

        assignment.LearningPathId = dto.NewLearningPathId;
        assignment.DueDate = dto.NewDueDate;
        assignment.ManagerNotes = dto.ManagerNotes;
        assignment.Status = "Assigned";
        assignment.ProgressPercent = 0;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAssignmentAsync(int managerId, int assignmentId)
    {
        var assignment = await _context.Assignments
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment == null) return false;

        if (assignment.Employee.ManagerId != managerId) return false;

        _context.Assignments.Remove(assignment);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task SendAssignmentEmailAsync(User employee, LearningPath learningPath, DateTime? dueDate, string? notes)
    {
        var courses = await _context.LearningPathCourses
            .Include(lpc => lpc.Course)
            .Where(lpc => lpc.LearningPathId == learningPath.Id)
            .OrderBy(lpc => lpc.Order)
            .ToListAsync();

        var modules = await _context.Modules
            .Where(m => m.LearningPathId == learningPath.Id)
            .OrderBy(m => m.Order)
            .ToListAsync();

        var emailBody = $@"
            <h2>New Learning Path Assignment</h2>
            <p>Dear {employee.FullName},</p>
            <p>You have been assigned a new learning path:</p>
            <h3>{learningPath.Title}</h3>
            <p>{learningPath.Description}</p>";

        if (courses.Any())
        {
            emailBody += "<h4>Courses Included:</h4><ul>";
            foreach (var lpc in courses)
            {
                emailBody += $"<li>{lpc.Course.Title}</li>";
            }
            emailBody += "</ul>";
        }
        else if (modules.Any())
        {
            emailBody += "<h4>Modules Included:</h4><ul>";
            foreach (var module in modules)
            {
                emailBody += $"<li>{module.Title}</li>";
            }
            emailBody += "</ul>";
        }

        if (dueDate.HasValue)
        {
            emailBody += $"<p><strong>Due Date:</strong> {dueDate.Value:MMMM dd, yyyy}</p>";
        }

        if (!string.IsNullOrEmpty(notes))
        {
            emailBody += $"<p><strong>Manager Notes:</strong> {notes}</p>";
        }

        emailBody += "<p>Please log in to the Learning Management System to start your learning journey.</p><p>Best regards,<br>Learning Management Team</p>";

        await _emailService.SendEmailAsync(employee.Email, $"New Learning Path Assignment: {learningPath.Title}", emailBody);
    }
}
