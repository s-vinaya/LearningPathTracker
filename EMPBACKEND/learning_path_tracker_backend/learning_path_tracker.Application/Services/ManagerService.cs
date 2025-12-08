using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Data.Repositories;
using learning_path_tracker.Database;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Application.Services;

public class ManagerService : IManagerService
{
    private readonly IUserRepository _userRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILearningPathRepository _learningPathRepository;
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public ManagerService(
        IUserRepository userRepository,
        IEnrollmentRepository enrollmentRepository,
        ILearningPathRepository learningPathRepository,
        AppDbContext context,
        EmailService emailService)
    {
        _userRepository = userRepository;
        _enrollmentRepository = enrollmentRepository;
        _learningPathRepository = learningPathRepository;
        _context = context;
        _emailService = emailService;
    }

    public async Task<object> GetDashboardStatsAsync(int managerId)
    {
        var users = await _userRepository.GetAllAsync();
        var manager = users.FirstOrDefault(u => u.Id == managerId);
        if (manager == null) return new { TotalMembers = 0, Active = 0, InTraining = 0 };
        
        var teamMembers = users.Where(u => u.Role?.ToLower() == "employee" && u.ManagerId == managerId && u.IsActive).ToList();
        var departmentUserIds = teamMembers.Select(u => u.Id).ToList();
        
        var assignments = await _context.Assignments
            .Where(a => departmentUserIds.Contains(a.EmployeeId))
            .ToListAsync();

        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => departmentUserIds.Contains(e.UserId))
            .ToListAsync();

        var activeCount = enrollments.Select(e => e.UserId).Distinct().Count();
        var inTrainingCount = assignments.Where(a => a.Status == "InProgress" || a.Status == "Assigned").Select(a => a.EmployeeId).Distinct().Count();
        var completedPaths = assignments.Count(a => a.Status == "Completed");
        var activePaths = assignments
            .Where(a => a.Status != "Completed")
            .Select(a => a.LearningPathId)
            .Distinct()
            .Count();

        var totalEnrollments = enrollments.Count;
        var completedEnrollments = enrollments.Count(e => e.Progress >= 80 && (!e.Course.QuizId.HasValue || e.QuizPassed));
        var completionRate = totalEnrollments > 0 ? (completedEnrollments * 100 / totalEnrollments) : 0;

        var totalWeeklyMinutes = 0;
        foreach (var enrollment in enrollments)
        {
            if (!string.IsNullOrEmpty(enrollment.Course.VideoDuration))
            {
                var parts = enrollment.Course.VideoDuration.Split(':');
                if (parts.Length >= 2 && int.TryParse(parts[0], out int mins))
                {
                    totalWeeklyMinutes += (mins * enrollment.Progress) / 100;
                }
            }
        }
        var weeklyHours = totalWeeklyMinutes >= 60 ? $"{totalWeeklyMinutes / 60}h" : $"{totalWeeklyMinutes}m";

        return new
        {
            TotalMembers = teamMembers.Count,
            Active = activeCount,
            InTraining = inTrainingCount,
            InTrainingAssignments = assignments.Count(a => a.Status == "InProgress" || a.Status == "Assigned"),
            CompletedPaths = completedPaths,
            ActivePaths = activePaths,
            CompletionRate = completionRate,
            WeeklyHours = weeklyHours
        };
    }

    public async Task<object> GetTeamMembersAsync(int managerId)
    {
        var users = await _userRepository.GetAllAsync();
        var manager = users.FirstOrDefault(u => u.Id == managerId);
        if (manager == null) return new List<object>();
        
        var teamMembers = users.Where(u => u.Role?.ToLower() == "employee" && u.ManagerId == managerId && u.IsActive).ToList();

        var members = new List<object>();
        foreach (var user in teamMembers)
        {
            var userEnrollments = await _enrollmentRepository.GetByUserIdAsync(user.Id);
            var totalProgress = userEnrollments.Any() ? userEnrollments.Average(e => e.Progress) : 0;
            var status = user.IsActive ? "Active" : "Inactive";

            members.Add(new
            {
                Id = user.Id,
                Name = user.FullName,
                Role = user.JobTitle ?? "Employee",
                Department = user.Department ?? "General",
                Status = status,
                Progress = (int)totalProgress
            });
        }

        return members;
    }

    public async Task<object> GetLearningPathsAsync()
    {
        var learningPaths = await _learningPathRepository.GetAllAsync();

        return learningPaths.Select(lp => {
            var courses = lp.LearningPathCourses
                .OrderBy(lpc => lpc.Order)
                .Select(lpc => new {
                    Title = lpc.Course?.Title ?? "Unknown",
                    Duration = lpc.Course?.VideoDuration ?? "0:00"
                }).ToList();
            
            var totalMinutes = courses.Any() ? courses.Sum(c => ParseDuration(c.Duration)) : 0;
            var calculatedDuration = totalMinutes > 0 ? totalMinutes / 60.0 : lp.EstimatedDuration;

            return new
            {
                Id = lp.Id,
                Title = lp.Title,
                Description = lp.Description,
                EstimatedDuration = Math.Round(calculatedDuration, 1),
                DifficultyLevel = lp.DifficultyLevel,
                Courses = courses,
                CourseCount = courses.Count
            };
        }).ToList();
    }

    private int ParseDuration(string duration)
    {
        if (string.IsNullOrEmpty(duration)) return 0;
        var parts = duration.Split(':');
        if (parts.Length == 2 && int.TryParse(parts[0], out int minutes) && int.TryParse(parts[1], out int seconds))
            return minutes + (seconds / 60);
        return 0;
    }

    public async Task<object> GetReportsAsync(int managerId)
    {
        var users = await _userRepository.GetAllAsync();
        var manager = users.FirstOrDefault(u => u.Id == managerId);
        if (manager == null) return new { LearningPaths = new List<object>() };
        
        var departmentUserIds = users.Where(u => u.ManagerId == managerId).Select(u => u.Id).ToList();
        
        var learningPaths = await _learningPathRepository.GetAllAsync();
        var assignments = await _context.Assignments
            .Where(a => departmentUserIds.Contains(a.EmployeeId))
            .ToListAsync();

        var pathReports = new List<object>();
        foreach (var path in learningPaths)
        {
            var pathAssignments = assignments.Where(a => a.LearningPathId == path.Id).ToList();

            var enrolled = pathAssignments.Count;
            var completed = pathAssignments.Count(a => a.Status == "Completed");
            var inProgress = pathAssignments.Count(a => a.Status == "InProgress" || a.Status == "Assigned");
            var completionRate = enrolled > 0 ? (completed * 100 / enrolled) : 0;

            pathReports.Add(new
            {
                Name = path.Title,
                Icon = "fas fa-book",
                Enrolled = enrolled,
                Completed = completed,
                InProgress = inProgress,
                CompletionRate = completionRate,
                AvgTime = $"{path.EstimatedDuration}h"
            });
        }

        var activeAssignments = assignments.Where(a => a.Status == "InProgress" || a.Status == "Assigned").Count();
        var completedAssignments = assignments.Where(a => a.Status == "Completed").Count();
        var totalAssignments = assignments.Count();
        var successRate = totalAssignments > 0 ? (completedAssignments * 100 / totalAssignments) : 0;

        return new
        {
            LearningPaths = pathReports,
            Metrics = new
            {
                ActiveLearners = assignments.Select(a => a.EmployeeId).Distinct().Count(),
                HoursCompleted = completedAssignments * 5,
                PathsInProgress = activeAssignments,
                SuccessRate = successRate
            }
        };
    }

    public async Task<object> GetWeeklyHoursAsync(int managerId)
    {
        var users = await _userRepository.GetAllAsync();
        var manager = users.FirstOrDefault(u => u.Id == managerId);
        if (manager == null) return new { Hours = new List<object>() };
        
        var teamMembers = users.Where(u => u.Role?.ToLower() == "employee" && u.ManagerId == managerId && u.IsActive).ToList();
        var departmentUserIds = teamMembers.Select(u => u.Id).ToList();

        var weekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek + 1);
        var assignments = await _context.Assignments
            .Where(a => departmentUserIds.Contains(a.EmployeeId))
            .ToListAsync();

        var employeeHours = new List<object>();
        double totalWeeklyHours = 0;
        int goalsCompleted = 0;

        foreach (var user in teamMembers)
        {
            var userAssignments = assignments.Where(a => a.EmployeeId == user.Id).ToList();
            var inProgressAssignments = userAssignments.Where(a => a.Status == "InProgress" && a.ProgressPercent > 0).ToList();
            
            var hoursPerDay = inProgressAssignments.Count > 0 ? inProgressAssignments.Sum(a => a.ProgressPercent) / 100.0 * 0.5 : 0;
            var mon = hoursPerDay > 0 ? $"{hoursPerDay:F1}h" : "0h";
            var tue = hoursPerDay > 0 ? $"{hoursPerDay:F1}h" : "0h";
            var wed = hoursPerDay > 0 ? $"{hoursPerDay:F1}h" : "0h";
            var thu = hoursPerDay > 0 ? $"{hoursPerDay:F1}h" : "0h";
            var fri = hoursPerDay > 0 ? $"{hoursPerDay:F1}h" : "0h";
            
            var weeklyTotal = hoursPerDay * 5;
            totalWeeklyHours += weeklyTotal;
            var status = weeklyTotal >= 8 ? "On Track" : weeklyTotal > 0 ? "In Progress" : "Not Started";
            if (weeklyTotal >= 8) goalsCompleted++;

            employeeHours.Add(new
            {
                Name = user.FullName,
                Mon = mon,
                Tue = tue,
                Wed = wed,
                Thu = thu,
                Fri = fri,
                Total = $"{weeklyTotal:F1}h",
                Status = status
            });
        }

        var avgHours = teamMembers.Count > 0 ? (int)(totalWeeklyHours / teamMembers.Count) : 0;
        var targetHours = teamMembers.Count * 8;
        var targetAchievement = targetHours > 0 ? (int)((totalWeeklyHours / targetHours) * 100) : 0;

        return new
        {
            Hours = employeeHours,
            Stats = new
            {
                TotalHours = (int)totalWeeklyHours,
                AvgHours = avgHours,
                TargetAchievement = targetAchievement,
                GoalsCompleted = goalsCompleted
            }
        };
    }

    public async Task<object> GetManagerProfileAsync(int managerId)
    {
        var manager = await _userRepository.GetByIdAsync(managerId);
        if (manager == null)
            return new { error = "Manager not found" };

        string? profileImageBase64 = null;
        if (manager.ProfileImage != null && manager.ProfileImage.Length > 0)
        {
            profileImageBase64 = Convert.ToBase64String(manager.ProfileImage);
        }

        return new
        {
            Id = manager.Id,
            Name = manager.FullName,
            Email = manager.Email,
            Department = manager.Department,
            JobTitle = manager.JobTitle,
            PhoneNumber = manager.PhoneNumber,
            ProfileImageBase64 = profileImageBase64
        };
    }

    public async Task<object> GetAssignmentsAsync(int managerId)
    {
        // For now, return empty list since Assignment table needs migration
        return new { assignments = new List<object>() };
    }

    public async Task<object> GetAllApprovalsAsync(int managerId)
    {
        var approvals = await _context.Approvals
            .Include(a => a.Employee)
            .Where(a => a.ManagerId == managerId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FullName,
                EmployeeEmail = a.Employee.Email,
                Type = a.Type,
                CreatedAt = a.CreatedAt,
                Status = a.Status,
                RequestDetails = a.RequestDetails,
                ReviewerComments = a.ReviewerComments,
                ReviewedAt = a.ReviewedAt
            })
            .ToListAsync();

        return approvals;
    }

    public async Task<object> GetPendingApprovalsAsync(int managerId)
    {
        var approvals = await _context.Approvals
            .Include(a => a.Employee)
            .Where(a => a.ManagerId == managerId && a.Status == "Pending")
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FullName,
                EmployeeEmail = a.Employee.Email,
                Type = a.Type,
                CreatedAt = a.CreatedAt,
                Status = a.Status,
                RequestDetails = a.RequestDetails,
                ReviewerComments = a.ReviewerComments
            })
            .ToListAsync();

        return approvals;
    }

    public async Task<bool> ApproveRequestAsync(int approvalId, int managerId, string? comments)
    {
        var approval = await _context.Approvals
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.Id == approvalId);
        
        if (approval == null || approval.ManagerId != managerId)
            return false;

        approval.Status = "Approved";
        approval.ReviewedAt = DateTime.UtcNow;
        approval.ReviewerComments = comments;

        if (approval.Type == "CertificateRequest" && !string.IsNullOrEmpty(approval.Payload))
        {
            var payload = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(approval.Payload);
            var courseId = payload.TryGetProperty("courseId", out var cId) ? cId.GetInt32() : 0;
            var learningPathId = payload.TryGetProperty("learningPathId", out var lpId) ? lpId.GetInt32() : 0;
            
            var manager = await _userRepository.GetByIdAsync(managerId);
            var employee = approval.Employee;
            
            if (courseId > 0)
            {
                var course = await _context.Courses.FindAsync(courseId);
                var quizAttempts = await _context.QuizAttempts
                    .Where(qa => qa.UserId == approval.EmployeeId && qa.Quiz.CourseId == courseId)
                    .ToListAsync();
                var averageScore = quizAttempts.Any() ? quizAttempts.Max(qa => qa.Score) : 0;
                
                var certificate = new Domain.Entities.Certificate
                {
                    CertificateId = $"CERT-{DateTime.UtcNow:yyyyMMddHHmmss}-{approval.EmployeeId:D6}-C{courseId:D4}",
                    UserId = approval.EmployeeId,
                    EmployeeName = employee.FullName,
                    CourseId = courseId,
                    CourseName = course?.Title,
                    ManagerName = manager?.FullName,
                    AverageScore = averageScore,
                    CertificateType = "Course",
                    IssuedAt = DateTime.UtcNow
                };
                _context.Certificates.Add(certificate);
            }
            else if (learningPathId > 0)
            {
                var learningPath = await _context.LearningPaths.FindAsync(learningPathId);
                var courseIds = await _context.Set<Domain.Entities.LearningPathCourse>()
                    .Where(lpc => lpc.LearningPathId == learningPathId)
                    .Select(lpc => lpc.CourseId)
                    .ToListAsync();
                var quizAttempts = await _context.QuizAttempts
                    .Where(qa => qa.UserId == approval.EmployeeId && courseIds.Contains(qa.Quiz.CourseId ?? 0))
                    .ToListAsync();
                var averageScore = quizAttempts.Any() ? quizAttempts.Average(qa => qa.Score) : 0;
                
                var certificate = new Domain.Entities.Certificate
                {
                    CertificateId = $"CERT-{DateTime.UtcNow:yyyyMMddHHmmss}-{approval.EmployeeId:D6}-LP{learningPathId:D4}",
                    UserId = approval.EmployeeId,
                    EmployeeName = employee.FullName,
                    LearningPathId = learningPathId,
                    LearningPathName = learningPath?.Title,
                    ManagerName = manager?.FullName,
                    AverageScore = averageScore,
                    CertificateType = "LearningPath",
                    IssuedAt = DateTime.UtcNow
                };
                _context.Certificates.Add(certificate);
            }
        }
        else if (approval.Type == "CourseRequest" && !string.IsNullOrEmpty(approval.RequestDetails))
        {
            var details = approval.RequestDetails.Split('|');
            var courseIdStr = details.FirstOrDefault(d => d.StartsWith("CourseId:"))?.Replace("CourseId:", "").Trim();
            
            if (!string.IsNullOrEmpty(courseIdStr) && int.TryParse(courseIdStr, out int courseId))
            {
                var existingEnrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.UserId == approval.EmployeeId && e.CourseId == courseId);
                
                if (existingEnrollment == null)
                {
                    var enrollment = new Domain.Entities.Enrollment
                    {
                        UserId = approval.EmployeeId,
                        CourseId = courseId,
                        EnrolledAt = DateTime.UtcNow,
                        Progress = 0,
                        QuizPassed = false
                    };
                    _context.Enrollments.Add(enrollment);
                }
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectRequestAsync(int approvalId, int managerId, string? comments)
    {
        var approval = await _context.Approvals.FindAsync(approvalId);
        if (approval == null || approval.ManagerId != managerId)
            return false;

        approval.Status = "Rejected";
        approval.ReviewedAt = DateTime.UtcNow;
        approval.ReviewerComments = comments;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<object?> GetEmployeeDetailsAsync(int employeeId, int managerId)
    {
        var employee = await _userRepository.GetByIdAsync(employeeId);
        if (employee == null || employee.ManagerId != managerId)
            return null;

        var enrollments = await _enrollmentRepository.GetByUserIdAsync(employeeId);
        var completedCourses = enrollments.Count(e => e.CompletedAt != null);
        var overallProgress = enrollments.Any() ? (int)enrollments.Average(e => e.Progress) : 0;

        var assignments = await _context.Assignments
            .Include(a => a.LearningPath)
            .Where(a => a.EmployeeId == employeeId && a.Status != "Completed")
            .ToListAsync();

        var assignedPaths = assignments.Select(a => new {
            Title = a.LearningPath.Title,
            Progress = a.ProgressPercent,
            DueDate = a.DueDate,
            Status = a.Status
        }).ToList();

        string? profilePicture = null;
        if (employee.ProfileImage != null && employee.ProfileImage.Length > 0)
        {
            profilePicture = $"data:image/jpeg;base64,{Convert.ToBase64String(employee.ProfileImage)}";
        }

        return new
        {
            Name = employee.FullName,
            Email = employee.Email,
            Role = employee.JobTitle ?? "Employee",
            Department = employee.Department,
            Status = employee.IsActive ? "Active" : "Inactive",
            ProfilePicture = profilePicture,
            OverallProgress = overallProgress,
            EnrolledCourses = enrollments.Count,
            CompletedCourses = completedCourses,
            CertificatesEarned = completedCourses,
            LearningPaths = assignedPaths
        };
    }

    public async Task<object?> GetEmployeeProgressAsync(int employeeId, int managerId)
    {
        var employee = await _userRepository.GetByIdAsync(employeeId);
        if (employee == null || employee.ManagerId != managerId)
            return null;

        var enrollments = await _enrollmentRepository.GetByUserIdAsync(employeeId);
        var courses = new List<object>();
        int totalTimeSpent = 0;
        double totalQuizScore = 0;
        int quizCount = 0;

        foreach (var enrollment in enrollments)
        {
            var course = await _context.Courses.FindAsync(enrollment.CourseId);
            if (course != null)
            {
                courses.Add(new
                {
                    Title = course.Title,
                    Progress = enrollment.Progress,
                    TimeSpent = $"{enrollment.Progress / 10}h",
                    QuizScore = enrollment.Progress > 50 ? $"{enrollment.Progress}%" : "N/A"
                });
                totalTimeSpent += enrollment.Progress / 10;
                if (enrollment.Progress > 50)
                {
                    totalQuizScore += enrollment.Progress;
                    quizCount++;
                }
            }
        }

        var completedCount = enrollments.Count(e => e.CompletedAt != null);
        var completionRate = enrollments.Any() ? (completedCount * 100 / enrollments.Count) : 0;

        return new
        {
            Courses = courses,
            TotalTimeSpent = $"{totalTimeSpent}h",
            AvgQuizScore = quizCount > 0 ? $"{(int)(totalQuizScore / quizCount)}%" : "N/A",
            CompletionRate = completionRate
        };
    }

    public async Task<bool> SendReminderAsync(int employeeId, int managerId)
    {
        try
        {
            var employee = await _userRepository.GetByIdAsync(employeeId);
            if (employee == null || employee.ManagerId != managerId)
                return false;

            var enrollments = await _enrollmentRepository.GetByUserIdAsync(employeeId);
            var pendingCourses = new List<string>();
            var overdueAssessments = new List<string>();

            foreach (var enrollment in enrollments)
            {
                if (enrollment.CompletedAt == null)
                {
                    var course = await _context.Courses.FindAsync(enrollment.CourseId);
                    if (course != null)
                    {
                        pendingCourses.Add(course.Title);
                        if (enrollment.EnrolledAt.AddDays(30) < DateTime.UtcNow)
                        {
                            overdueAssessments.Add(course.Title);
                        }
                    }
                }
            }

            var emailBody = $@"
                <h2>Learning Path Reminder</h2>
                <p>Dear {employee.FullName},</p>
                <p>This is a reminder about your learning activities:</p>";

            if (pendingCourses.Any())
            {
                emailBody += "<h3>Pending Courses:</h3><ul>";
                foreach (var course in pendingCourses)
                {
                    emailBody += $"<li>{course}</li>";
                }
                emailBody += "</ul>";
            }
            else
            {
                emailBody += "<p>You have no pending courses at the moment. Keep up the good work!</p>";
            }

            if (overdueAssessments.Any())
            {
                emailBody += "<h3>Overdue Assessments:</h3><ul>";
                foreach (var assessment in overdueAssessments)
                {
                    emailBody += $"<li>{assessment} - Due date has passed</li>";
                }
                emailBody += "</ul>";
            }

            emailBody += "<p>Please complete these at your earliest convenience.</p><p>Best regards,<br>Learning Management Team</p>";

            await _emailService.SendEmailAsync(employee.Email, "Learning Path Reminder", emailBody);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending reminder: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateManagerProfileAsync(int managerId, UpdateProfileDto dto)
    {
        var manager = await _userRepository.GetByIdAsync(managerId);
        if (manager == null) return false;

        if (!string.IsNullOrEmpty(dto.FullName)) manager.FullName = dto.FullName;
        if (!string.IsNullOrEmpty(dto.Email)) manager.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.Department)) manager.Department = dto.Department;
        if (!string.IsNullOrEmpty(dto.Team)) manager.JobTitle = dto.Team;

        await _context.SaveChangesAsync();
        return true;
    }

    public Task<bool> UpdateNotificationSettingsAsync(int managerId, NotificationSettingsDto dto)
    {
        // Store in database or cache - for now just return success
        return Task.FromResult(true);
    }

    public Task<bool> UpdateTeamSettingsAsync(int managerId, TeamSettingsDto dto)
    {
        // Store in database or cache - for now just return success
        return Task.FromResult(true);
    }

    public Task<NotificationSettingsDto> GetNotificationSettingsAsync(int managerId)
    {
        // Retrieve from database or return defaults
        return Task.FromResult(new NotificationSettingsDto
        {
            EmailNotifications = true,
            WeeklyProgressReports = true,
            CourseCompletionAlerts = false,
            AssignmentReminders = true,
            TeamUpdates = false,
            SystemNotifications = false
        });
    }

    public Task<TeamSettingsDto> GetTeamSettingsAsync(int managerId)
    {
        // Retrieve from database or return defaults
        return Task.FromResult(new TeamSettingsDto
        {
            DefaultLearningHours = 8,
            AssignmentDeadline = 14,
            ReminderFrequency = "Weekly",
            AutoAssignNewPaths = "Disabled"
        });
    }

    public async Task<bool> UpdateProfileImageAsync(int managerId, byte[] imageBytes)
    {
        var manager = await _userRepository.GetByIdAsync(managerId);
        if (manager == null) return false;

        manager.ProfileImage = imageBytes;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<object>> GetPendingCertificateRequestsAsync(int managerId)
    {
        var manager = await _userRepository.GetByIdAsync(managerId);
        if (manager == null) return new List<object>();

        var departmentEmployeeIds = (await _userRepository.GetAllAsync())
            .Where(u => u.Department == manager.Department && u.Role == "Employee")
            .Select(u => u.Id)
            .ToList();

        var requests = await _context.CertificateRequests
            .Include(r => r.Employee)
            .Include(r => r.Course)
            .Include(r => r.LearningPath)
            .Where(r => departmentEmployeeIds.Contains(r.EmployeeId) && r.Status == "Pending")
            .OrderBy(r => r.RequestedDate)
            .ToListAsync();

        return requests.Select(r => new
        {
            r.Id,
            r.EmployeeId,
            EmployeeName = r.Employee.FullName,
            r.CourseId,
            CourseName = r.Course?.Title,
            r.LearningPathId,
            LearningPathName = r.LearningPath?.Title,
            r.RequestType,
            r.CompletedDate,
            r.RequestedDate,
            r.Status
        }).Cast<object>().ToList();
    }

    public async Task<object> ApproveCertificateRequestAsync(int requestId, int managerId)
    {
        var request = await _context.CertificateRequests
            .Include(r => r.Employee)
            .Include(r => r.Course)
            .Include(r => r.LearningPath)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request == null)
            throw new Exception($"Certificate request {requestId} not found");
        
        if (request.Status != "Pending")
            throw new Exception($"Request already {request.Status}");

        var manager = await _userRepository.GetByIdAsync(managerId);
        var employee = request.Employee;

        if (manager == null)
            throw new Exception("Manager not found");
        
        if (employee.Department != manager.Department)
            throw new Exception("Unauthorized");

        var managerName = manager.FullName;
        var averageScore = 0.0;

        if (request.RequestType == "Course" && request.CourseId.HasValue)
        {
            var quizAttempts = await _context.QuizAttempts
                .Where(qa => qa.UserId == request.EmployeeId && qa.Quiz.CourseId == request.CourseId)
                .ToListAsync();
            averageScore = quizAttempts.Any() ? quizAttempts.Max(qa => qa.Score) : 0;
        }
        else if (request.RequestType == "LearningPath" && request.LearningPathId.HasValue)
        {
            var courseIds = await _context.Set<Domain.Entities.LearningPathCourse>()
                .Where(lpc => lpc.LearningPathId == request.LearningPathId)
                .Select(lpc => lpc.CourseId)
                .ToListAsync();

            var quizAttempts = await _context.QuizAttempts
                .Where(qa => qa.UserId == request.EmployeeId && courseIds.Contains(qa.Quiz.CourseId ?? 0))
                .ToListAsync();
            averageScore = quizAttempts.Any() ? quizAttempts.Max(qa => qa.Score) : 0;
        }

        var certificateId = request.RequestType == "Course"
            ? $"CERT-{DateTime.UtcNow:yyyyMMddHHmmss}-{request.EmployeeId:D6}-C{request.CourseId:D4}"
            : $"CERT-{DateTime.UtcNow:yyyyMMddHHmmss}-{request.EmployeeId:D6}-LP{request.LearningPathId:D4}";

        var certificate = new Domain.Entities.Certificate
        {
            CertificateId = certificateId,
            UserId = request.EmployeeId,
            EmployeeName = employee.FullName,
            CourseId = request.CourseId,
            CourseName = request.Course?.Title,
            LearningPathId = request.LearningPathId,
            LearningPathName = request.LearningPath?.Title,
            ManagerName = managerName,
            AverageScore = averageScore,
            IssuedAt = DateTime.UtcNow,
            CertificateType = request.RequestType
        };

        _context.Certificates.Add(certificate);
        await _context.SaveChangesAsync();
        
        request.Status = "Approved";
        request.ReviewedByManagerId = managerId;
        request.ReviewedDate = DateTime.UtcNow;
        request.CertificateId = certificate.Id;
        await _context.SaveChangesAsync();

        var itemName = request.RequestType == "Course" ? request.Course?.Title : request.LearningPath?.Title;
        var emailBody = $@"
            <h2>🎉 Congratulations!</h2>
            <p>Dear {employee.FullName},</p>
            <p>We are pleased to inform you that your certificate request has been approved!</p>
            <h3>Certificate Details:</h3>
            <ul>
                <li><strong>Certificate ID:</strong> {certificateId}</li>
                <li><strong>{request.RequestType}:</strong> {itemName}</li>
                <li><strong>Average Score:</strong> {averageScore:F1}%</li>
                <li><strong>Issued Date:</strong> {DateTime.UtcNow:yyyy-MM-dd}</li>
                <li><strong>Approved By:</strong> {managerName}</li>
            </ul>
            <p>Your certificate is now available in your profile. Congratulations on your achievement!</p>
            <p>Best regards,<br>Learning Management Team</p>";

        await _emailService.SendEmailAsync(employee.Email, "Certificate Approved - Congratulations!", emailBody);

        return new { message = "Certificate approved and issued", certificateId };
    }

    public async Task<object> RejectCertificateRequestAsync(int requestId, int managerId, string rejectionReason)
    {
        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new Exception("Rejection reason is required");

        var request = await _context.CertificateRequests
            .Include(r => r.Employee)
            .Include(r => r.Course)
            .Include(r => r.LearningPath)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request == null || request.Status != "Pending")
            throw new Exception("Invalid request");

        var manager = await _userRepository.GetByIdAsync(managerId);
        var employee = request.Employee;

        if (employee.Department != manager?.Department)
            throw new Exception("Unauthorized");

        request.Status = "Rejected";
        request.ReviewedByManagerId = managerId;
        request.ReviewedDate = DateTime.UtcNow;
        request.RejectionReason = rejectionReason;
        await _context.SaveChangesAsync();

        var itemName = request.RequestType == "Course" ? request.Course?.Title : request.LearningPath?.Title;
        var emailBody = $@"
            <h2>Certificate Request Update</h2>
            <p>Dear {employee.FullName},</p>
            <p>Your certificate request for <strong>{itemName}</strong> has been reviewed.</p>
            <h3>Status: Not Approved</h3>
            <p><strong>Reason:</strong> {rejectionReason}</p>
            <p>Please address the feedback and resubmit your request when ready.</p>
            <p>Best regards,<br>{manager.FullName}<br>Learning Management Team</p>";

        await _emailService.SendEmailAsync(employee.Email, "Certificate Request Update", emailBody);

        return new { message = "Certificate request rejected", reason = rejectionReason };
    }
}
