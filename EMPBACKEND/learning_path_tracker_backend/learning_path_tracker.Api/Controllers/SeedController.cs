using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly AppDbContext _context;

    public SeedController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> SeedData()
    {
        if (_context.Users.Any())
            return Ok("Database already seeded");

        // Seed 100 Users
        var users = new List<User>();
        var departments = new[] { "Engineering", "Marketing", "Sales", "HR", "Operations" };
        for (int i = 1; i <= 100; i++)
        {
            users.Add(new User
            {
                Name = $"User {i}",
                FullName = $"User {i} Full",
                Email = $"user{i}@example.com",
                Role = i <= 5 ? "Admin" : i <= 20 ? "Manager" : "Employee",
                Department = departments[i % 5],
                JobTitle = $"Position {i}",
                IsActive = i <= 85,
                IsApproved = i <= 90,
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                LastLogin = i <= 85 ? DateTime.UtcNow.AddHours(-i) : null,
                Salt = "salt",
                PasswordHash = "hash"
            });
        }
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Seed 89 Courses (67 active, 22 draft)
        var courses = new List<Course>();
        var courseNames = new[] { "Advanced JavaScript Programming", "UI/UX Design Fundamentals", "Digital Marketing Strategy", "Python for Data Science", "React Development", "Node.js Backend", "SQL Database Design", "Cloud Computing AWS", "Machine Learning Basics", "Cybersecurity Essentials" };
        for (int i = 1; i <= 89; i++)
        {
            courses.Add(new Course
            {
                Title = i <= courseNames.Length ? courseNames[i - 1] : $"Course {i}",
                Description = $"Master concepts including closures, prototypes, and async programming",
                Instructor = $"Instructor {(i % 10) + 1}",
                DurationHours = (i % 8) + 4,
                IsActive = i <= 67,
                CreatedAt = DateTime.UtcNow.AddDays(-i * 2)
            });
        }
        _context.Courses.AddRange(courses);
        await _context.SaveChangesAsync();

        // Seed 1247 Enrollments
        var enrollments = new List<Enrollment>();
        for (int i = 0; i < 1247; i++)
        {
            var userId = users[i % users.Count].Id;
            var courseId = courses[i % courses.Count].Id;
            var isCompleted = i < 800;
            enrollments.Add(new Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                EnrolledAt = DateTime.UtcNow.AddDays(-(i % 90)),
                CompletedAt = isCompleted ? DateTime.UtcNow.AddDays(-(i % 60)) : null,
                Progress = isCompleted ? 100 : (i % 100)
            });
        }
        _context.Enrollments.AddRange(enrollments);
        await _context.SaveChangesAsync();

        // Seed 500 Certificates
        var certificates = new List<Certificate>();
        for (int i = 1; i <= 500; i++)
        {
            var user = users[i % users.Count];
            var course = courses[i % courses.Count];
            certificates.Add(new Certificate
            {
                CertificateId = $"CERT-{DateTime.UtcNow:yyyyMMdd}-{user.Id:D6}-C{course.Id:D4}-{i:D3}",
                UserId = user.Id,
                EmployeeName = user.FullName,
                CourseId = course.Id,
                CourseName = course.Title,
                ManagerName = "Manager Name",
                AverageScore = 75 + (i % 25),
                IssuedAt = DateTime.UtcNow.AddDays(-(i % 60)),
                CertificateType = "Course"
            });
        }
        _context.Certificates.AddRange(certificates);
        await _context.SaveChangesAsync();

        // Seed Learning Paths
        var learningPaths = new List<LearningPath>
        {
            new LearningPath { Title = "Full Stack Web Development", Description = "Complete journey from frontend to backend", IsActive = true, CreatedOn = DateTime.UtcNow.AddDays(-90) },
            new LearningPath { Title = "Data Science Fundamentals", Description = "Learn data analysis and ML", IsActive = true, CreatedOn = DateTime.UtcNow.AddDays(-80) },
            new LearningPath { Title = "Cloud Architecture", Description = "Master cloud computing", IsActive = true, CreatedOn = DateTime.UtcNow.AddDays(-70) },
            new LearningPath { Title = "Mobile App Development", Description = "Build mobile apps", IsActive = false, CreatedOn = DateTime.UtcNow.AddDays(-60) }
        };
        _context.LearningPaths.AddRange(learningPaths);
        await _context.SaveChangesAsync();

        // Seed Modules
        var modules = new List<Module>
        {
            new Module { Title = "HTML & CSS Fundamentals", Description = "Learn web basics", Order = 1, LearningPathId = learningPaths[0].Id },
            new Module { Title = "JavaScript Essentials", Description = "Master JS", Order = 2, LearningPathId = learningPaths[0].Id },
            new Module { Title = "React Development", Description = "Build UIs", Order = 3, LearningPathId = learningPaths[0].Id },
            new Module { Title = "Node.js Backend", Description = "Server-side", Order = 4, LearningPathId = learningPaths[0].Id },
            new Module { Title = "Database Design", Description = "Data modeling", Order = 5, LearningPathId = learningPaths[0].Id }
        };
        _context.Modules.AddRange(modules);
        await _context.SaveChangesAsync();

        // Seed Notifications
        var notifications = new List<Notification>();
        for (int i = 1; i <= 50; i++)
        {
            notifications.Add(new Notification
            {
                Title = $"Notification {i}",
                Message = $"This is notification message {i}",
                Type = i % 3 == 0 ? "Announcement" : i % 3 == 1 ? "Reminder" : "Alert",
                CreatedAt = DateTime.UtcNow.AddHours(-i),
                IsRead = i % 2 == 0
            });
        }
        _context.Notifications.AddRange(notifications);
        await _context.SaveChangesAsync();

        // Seed Activities
        var activities = new List<Activity>();
        for (int i = 1; i <= 100; i++)
        {
            var types = new[] { "user", "course", "certificate", "path", "system" };
            activities.Add(new Activity
            {
                Type = types[i % 5],
                Description = $"Activity {i}: User completed action",
                UserId = users[i % users.Count].Id,
                CreatedAt = DateTime.UtcNow.AddMinutes(-i * 10)
            });
        }
        _context.Activities.AddRange(activities);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Database seeded successfully", stats = new { users = users.Count, courses = courses.Count, enrollments = enrollments.Count, certificates = certificates.Count, learningPaths = learningPaths.Count, notifications = notifications.Count } });
    }

    [HttpPost("clear")]
    public async Task<IActionResult> ClearData()
    {
        _context.Activities.RemoveRange(_context.Activities);
        _context.Notifications.RemoveRange(_context.Notifications);
        _context.Modules.RemoveRange(_context.Modules);
        _context.LearningPaths.RemoveRange(_context.LearningPaths);
        _context.Certificates.RemoveRange(_context.Certificates);
        _context.Enrollments.RemoveRange(_context.Enrollments);
        _context.Courses.RemoveRange(_context.Courses);
        _context.Users.RemoveRange(_context.Users);
        await _context.SaveChangesAsync();
        return Ok("Database cleared successfully");
    }
}
