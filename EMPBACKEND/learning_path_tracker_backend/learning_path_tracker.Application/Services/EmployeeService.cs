using Microsoft.EntityFrameworkCore;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;

namespace learning_path_tracker.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;
    private readonly IPlatformSettingsService _settingsService;
    private readonly PointsCalculationService _pointsService;

    public EmployeeService(AppDbContext context, IPlatformSettingsService settingsService, PointsCalculationService pointsService)
    {
        _context = context;
        _settingsService = settingsService;
        _pointsService = pointsService;
    }

    public async Task<object> GetEmployeeLearningPathsAsync(int employeeId)
    {
        var assignments = await _context.Assignments
            .Where(a => a.EmployeeId == employeeId)
            .Include(a => a.LearningPath)
                .ThenInclude(lp => lp!.LearningPathCourses)
                    .ThenInclude(lpc => lpc.Course)
            .ToListAsync();

        return assignments.Select(assignment =>
        {
            var totalModules = assignment.LearningPath?.LearningPathCourses?.Count ?? 0;
            var completedModules = 0;
            int averageProgress = 0;
            
            if (assignment.LearningPath?.LearningPathCourses != null)
            {
                var courseIds = assignment.LearningPath.LearningPathCourses.Select(lpc => lpc.CourseId).ToList();
                var enrollments = _context.Enrollments
                    .Where(e => e.UserId == employeeId && courseIds.Contains(e.CourseId))
                    .ToList();
                    
                completedModules = enrollments.Count(e => e.CompletedAt != null);
                
                if (enrollments.Any())
                {
                    averageProgress = (int)enrollments.Average(e => e.Progress);
                }
            }

            return new
            {
                id = assignment.LearningPath?.Id ?? 0,
                name = assignment.LearningPath?.Title ?? "",
                description = assignment.LearningPath?.Description ?? "",
                status = averageProgress == 0 ? "Not Started" : averageProgress == 100 ? "Completed" : "In Progress",
                dueDate = assignment.DueDate,
                progress = averageProgress,
                totalModules = totalModules,
                completedModules = completedModules
            };
        }).ToList();
    }

    public async Task<IEnumerable<object>> GetAvailableCoursesAsync(int? userId = null)
    {
        var query = _context.Courses.Where(c => c.IsActive);

        if (userId.HasValue)
        {
            var enrolledCourseIds = await _context.Enrollments
                .Where(e => e.UserId == userId.Value)
                .Select(e => e.CourseId)
                .ToListAsync();

            query = query.Where(c => !enrolledCourseIds.Contains(c.Id));
        }

        return await query
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.Description,
                c.Instructor,
                c.DurationHours,
                c.Category,
                c.ThumbnailUrl,
                c.YouTubeUrl,
                EnrollmentCount = c.Enrollments.Count
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<object>> GetEnrolledCoursesAsync(int userId)
    {
        return await _context.Enrollments
            .Where(e => e.UserId == userId)
            .Include(e => e.Course)
            .Select(e => new
            {
                Id = e.Course.Id,
                Title = e.Course.Title,
                Description = e.Course.Description,
                Instructor = e.Course.Instructor,
                DurationHours = e.Course.DurationHours,
                Category = e.Course.Category,
                ThumbnailUrl = e.Course.ThumbnailUrl,
                YouTubeUrl = e.Course.YouTubeUrl,
                Progress = e.Progress,
                EnrolledAt = e.EnrolledAt,
                HasQuiz = e.Course.QuizId.HasValue,
                QuizId = e.Course.QuizId,
                QuizPassed = e.QuizPassed,
                QuizAvailable = e.Progress >= 80,
                CanGenerateCertificate = e.Progress >= 100 && (!e.Course.QuizId.HasValue || e.QuizPassed)
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<object>> GetAvailableLearningPathsAsync()
    {
        return await _context.LearningPaths
            .Where(lp => lp.IsActive)
            .Include(lp => lp.Modules)
            .Select(lp => new
            {
                lp.Id,
                lp.Title,
                lp.Description,
                lp.Level,
                lp.IsActive,
                Modules = lp.Modules.Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.Description,
                    m.Order
                }).OrderBy(m => m.Order).ToList()
            })
            .ToListAsync();
    }

    public async Task<object?> GetCourseDetailsAsync(int userId, int courseId)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (enrollment != null)
        {
            return new
            {
                id = enrollment.Course.Id,
                title = enrollment.Course.Title,
                description = enrollment.Course.Description,
                instructor = enrollment.Course.Instructor,
                durationHours = enrollment.Course.DurationHours,
                category = enrollment.Course.Category,
                thumbnailUrl = enrollment.Course.ThumbnailUrl,
                youTubeUrl = enrollment.Course.YouTubeUrl,
                progress = enrollment.Progress,
                hasQuiz = enrollment.Course.QuizId.HasValue,
                quizPassed = enrollment.QuizPassed
            };
        }

        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
            return null;

        return new
        {
            id = course.Id,
            title = course.Title,
            description = course.Description,
            instructor = course.Instructor,
            durationHours = course.DurationHours,
            category = course.Category,
            thumbnailUrl = course.ThumbnailUrl,
            youTubeUrl = course.YouTubeUrl,
            progress = 0,
            hasQuiz = course.QuizId.HasValue,
            quizPassed = false
        };
    }

    public async Task<bool> EnrollInCourseAsync(int userId, int courseId)
    {
        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (existingEnrollment != null)
            return false;

        var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
            return false;

        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow,
            Progress = 0,
            QuizPassed = false
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EnrollInLearningPathAsync(int userId, int learningPathId)
    {
        var existingAssignment = await _context.Assignments
            .FirstOrDefaultAsync(a => a.EmployeeId == userId && a.LearningPathId == learningPathId);

        if (existingAssignment != null)
            return false;

        var assignment = new Assignment
        {
            EmployeeId = userId,
            LearningPathId = learningPathId,
            AssignedByManagerId = userId,
            AssignedDate = DateTime.UtcNow,
            Status = "Assigned",
            ProgressPercent = 0
        };

        _context.Assignments.Add(assignment);
        
        var learningPath = await _context.LearningPaths
            .Include(lp => lp.LearningPathCourses)
            .Include(lp => lp.Modules)
            .FirstOrDefaultAsync(lp => lp.Id == learningPathId);
            
        if (learningPath != null)
        {
            var courseIds = new List<int>();
            
            if (learningPath.LearningPathCourses?.Any() == true)
            {
                courseIds = learningPath.LearningPathCourses.Select(lpc => lpc.CourseId).ToList();
            }
            else if (learningPath.Modules?.Any() == true)
            {
                courseIds = learningPath.Modules.Where(m => m.CourseId.HasValue).Select(m => m.CourseId!.Value).ToList();
            }
            
            foreach (var courseId in courseIds)
            {
                var existingEnrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
                    
                if (existingEnrollment == null)
                {
                    var enrollment = new Enrollment
                    {
                        UserId = userId,
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

    public async Task<IEnumerable<object>> GetLearningPathCoursesAsync(int pathId)
    {
        var courses = await _context.Set<LearningPathCourse>()
            .Where(lpc => lpc.LearningPathId == pathId)
            .Include(lpc => lpc.Course)
            .OrderBy(lpc => lpc.Order)
            .ToListAsync();

        return courses.Select(lpc => new
        {
            id = lpc.Course.Id,
            title = lpc.Course.Title,
            description = lpc.Course.Description,
            instructor = lpc.Course.Instructor,
            durationHours = lpc.Course.DurationHours,
            category = lpc.Course.Category,
            thumbnailUrl = lpc.Course.ThumbnailUrl,
            youTubeUrl = lpc.Course.YouTubeUrl,
            order = lpc.Order,
            isLocked = false,
            hasQuiz = lpc.Course.QuizId.HasValue,
            quizId = lpc.Course.QuizId
        }).ToList();
    }

    public async Task<IEnumerable<object>> GetLearningPathCoursesAsync(int userId, int pathId)
    {
        var courses = await _context.Set<LearningPathCourse>()
            .Where(lpc => lpc.LearningPathId == pathId)
            .Include(lpc => lpc.Course)
            .OrderBy(lpc => lpc.Order)
            .ToListAsync();

        var courseIds = courses.Select(c => c.CourseId).ToList();
        var enrollments = await _context.Enrollments
            .Where(e => e.UserId == userId && courseIds.Contains(e.CourseId))
            .ToDictionaryAsync(e => e.CourseId);

        var quizzes = await _context.Quizzes
            .Where(q => q.LearningPathId == pathId && q.IsActive)
            .Include(q => q.Attempts.Where(a => a.UserId == userId))
            .ToListAsync();

        var result = new List<object>();
        for (int i = 0; i < courses.Count; i++)
        {
            var lpc = courses[i];
            var isLocked = false;
            var unlockReason = "";

            if (i > 0)
            {
                var prevCourse = courses[i - 1];
                var prevEnrollment = enrollments.GetValueOrDefault(prevCourse.CourseId);
                
                var prevCourseQuiz = quizzes.FirstOrDefault(q => q.CourseId == prevCourse.CourseId);
                
                if (prevCourseQuiz != null)
                {
                    var quizPassed = prevCourseQuiz.Attempts.Any(a => a.Passed);
                    if (!quizPassed)
                    {
                        isLocked = true;
                        unlockReason = "Complete and pass the quiz of the previous course";
                    }
                }
                else if (prevEnrollment == null || prevEnrollment.Progress < 80)
                {
                    isLocked = true;
                    unlockReason = "Complete 80% of the previous course";
                }
            }

            var enrollment = enrollments.GetValueOrDefault(lpc.CourseId);
            result.Add(new
            {
                id = lpc.Course.Id,
                title = lpc.Course.Title,
                description = lpc.Course.Description,
                instructor = lpc.Course.Instructor,
                durationHours = lpc.Course.DurationHours,
                category = lpc.Course.Category,
                thumbnailUrl = lpc.Course.ThumbnailUrl,
                youTubeUrl = lpc.Course.YouTubeUrl,
                order = lpc.Order,
                isLocked = isLocked,
                unlockReason = unlockReason,
                progress = enrollment?.Progress ?? 0,
                hasQuiz = lpc.Course.QuizId.HasValue,
                quizId = lpc.Course.QuizId,
                quizPassed = enrollment?.QuizPassed ?? false
            });
        }

        return result;
    }

    public async Task UpdateCourseProgressAsync(int userId, int courseId, int progress)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (enrollment != null)
        {
            var oldProgress = enrollment.Progress;
            enrollment.Progress = Math.Min(progress, 100);
            if (progress >= 80 && enrollment.CompletedAt == null)
            {
                enrollment.CompletedAt = DateTime.UtcNow;
                await _pointsService.AwardPointsForCourseCompletion(userId, courseId);
            }
            
            if (progress >= 80)
            {
                var hasQuiz = enrollment.Course.QuizId.HasValue;
                var canGenerateCert = !hasQuiz || enrollment.QuizPassed;
                
                if (canGenerateCert)
                {
                    var settings = await _settingsService.GetSettingsAsync();
                    var existingCert = await _context.Certificates
                        .FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId && c.CertificateType == "Course");
                    var existingApproval = await _context.Approvals
                        .FirstOrDefaultAsync(a => a.EmployeeId == userId && a.Type == "CertificateRequest" && a.Status == "Pending" && a.Payload.Contains($"\"courseId\":{courseId}"));
                    
                    if (existingCert == null && existingApproval == null)
                    {
                        var user = await _context.Users.FindAsync(userId);
                        
                        if (settings.Certificates)
                        {
                            var quizAttempts = await _context.QuizAttempts
                                .Where(qa => qa.UserId == userId && qa.Quiz.CourseId == courseId)
                                .ToListAsync();
                            var averageScore = quizAttempts.Any() ? quizAttempts.Max(qa => qa.Score) : 0;
                            var manager = await _context.Users
                                .FirstOrDefaultAsync(u => u.Department == user.Department && u.Role == "Manager");

                            var certificate = new Certificate
                            {
                                CertificateId = $"CERT-{DateTime.UtcNow:yyyyMMddHHmmss}-{userId:D6}-C{courseId:D4}",
                                UserId = userId,
                                EmployeeName = user.FullName,
                                CourseId = courseId,
                                CourseName = enrollment.Course.Title,
                                ManagerName = manager?.FullName ?? "System",
                                AverageScore = averageScore,
                                CertificateType = "Course",
                                IssuedAt = DateTime.UtcNow
                            };
                            _context.Certificates.Add(certificate);
                        }
                        else
                        {
                            var manager = await _context.Users
                                .FirstOrDefaultAsync(u => u.Department == user.Department && u.Role == "Manager");
                            
                            if (manager != null)
                            {
                                var approval = new Approval
                                {
                                    EmployeeId = userId,
                                    ManagerId = manager.Id,
                                    Type = "CertificateRequest",
                                    RequestDetails = $"Course: {enrollment.Course.Title} (ID: {courseId})",
                                    Payload = System.Text.Json.JsonSerializer.Serialize(new { courseId, courseName = enrollment.Course.Title }),
                                    Status = "Pending",
                                    CreatedAt = DateTime.UtcNow
                                };
                                _context.Approvals.Add(approval);
                            }
                        }
                    }
                }
            }
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<object> RequestCourseCertificateAsync(int userId, int courseId)
    {
        var settings = await _settingsService.GetSettingsAsync();
        
        var existingRequest = await _context.CertificateRequests
            .FirstOrDefaultAsync(r => r.EmployeeId == userId && r.CourseId == courseId && r.Status == "Pending");

        if (existingRequest != null)
            throw new Exception("Certificate request already pending approval");

        var existingCertificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId && c.CertificateType == "Course");

        if (existingCertificate != null)
            throw new Exception("Certificate already issued");

        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (enrollment == null || enrollment.Progress < 80)
            throw new Exception("Course not completed");

        if (enrollment.Course.QuizId.HasValue && !enrollment.QuizPassed)
            throw new Exception("Quiz not passed");

        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");

        if (settings.Certificates)
        {
            var certificate = new Certificate
            {
                UserId = userId,
                CourseId = courseId,
                CertificateType = "Course",
                IssuedAt = DateTime.UtcNow
            };
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();

            return new
            {
                id = certificate.Id,
                message = "Certificate generated successfully",
                status = "Approved"
            };
        }
        else
        {
            var manager = await _context.Users
                .FirstOrDefaultAsync(u => u.Department == user.Department && u.Role == "Manager");
            
            if (manager == null)
                throw new Exception("No manager found for your department");
            
            var approval = new Domain.Entities.Approval
            {
                EmployeeId = userId,
                ManagerId = manager.Id,
                Type = "CertificateRequest",
                RequestDetails = $"Course: {enrollment.Course.Title} (ID: {courseId})",
                Payload = System.Text.Json.JsonSerializer.Serialize(new { courseId, courseName = enrollment.Course.Title }),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Approvals.Add(approval);
            await _context.SaveChangesAsync();

            return new
            {
                id = approval.Id,
                message = "Certificate request submitted for manager approval",
                status = "Pending"
            };
        }
    }

    public async Task<object> RequestLearningPathCertificateAsync(int userId, int learningPathId)
    {
        var settings = await _settingsService.GetSettingsAsync();
        
        var existingRequest = await _context.CertificateRequests
            .FirstOrDefaultAsync(r => r.EmployeeId == userId && r.LearningPathId == learningPathId && r.Status == "Pending");

        if (existingRequest != null)
            throw new Exception("Certificate request already pending approval");

        var existingCertificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.UserId == userId && c.LearningPathId == learningPathId && c.CertificateType == "LearningPath");

        if (existingCertificate != null)
            throw new Exception("Certificate already issued");

        var learningPath = await _context.LearningPaths
            .Include(lp => lp.LearningPathCourses)
            .ThenInclude(lpc => lpc.Course)
            .FirstOrDefaultAsync(lp => lp.Id == learningPathId);

        if (learningPath == null) throw new Exception("Learning path not found");

        var courseIds = learningPath.LearningPathCourses.Select(lpc => lpc.CourseId).ToList();
        var enrollments = await _context.Enrollments
            .Where(e => e.UserId == userId && courseIds.Contains(e.CourseId))
            .Include(e => e.Course)
            .ToListAsync();

        if (enrollments.Count != courseIds.Count)
            throw new Exception("Not all courses completed");

        foreach (var enrollment in enrollments)
        {
            if (enrollment.Progress < 80)
                throw new Exception($"Course '{enrollment.Course.Title}' not completed");
            if (enrollment.Course.QuizId.HasValue && !enrollment.QuizPassed)
                throw new Exception($"Quiz for course '{enrollment.Course.Title}' not passed");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");

        if (settings.Certificates)
        {
            var certificate = new Certificate
            {
                UserId = userId,
                LearningPathId = learningPathId,
                CertificateType = "LearningPath",
                IssuedAt = DateTime.UtcNow
            };
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();

            return new
            {
                id = certificate.Id,
                message = "Certificate generated successfully",
                status = "Approved"
            };
        }
        else
        {
            var manager = await _context.Users
                .FirstOrDefaultAsync(u => u.Department == user.Department && u.Role == "Manager");
            
            if (manager == null)
                throw new Exception("No manager found for your department");
            
            var approval = new Domain.Entities.Approval
            {
                EmployeeId = userId,
                ManagerId = manager.Id,
                Type = "CertificateRequest",
                RequestDetails = $"Learning Path: {learningPath.Title} (ID: {learningPathId})",
                Payload = System.Text.Json.JsonSerializer.Serialize(new { learningPathId, learningPathName = learningPath.Title }),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Approvals.Add(approval);
            await _context.SaveChangesAsync();

            return new
            {
                id = approval.Id,
                message = "Certificate request submitted for manager approval",
                status = "Pending"
            };
        }
    }
}
