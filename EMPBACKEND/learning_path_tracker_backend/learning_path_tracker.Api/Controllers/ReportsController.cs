using learning_path_tracker.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReportsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
    {
        var now = DateTime.UtcNow.AddHours(5).AddMinutes(30);
        var last30Days = now.AddDays(-30);
        var previous30Days = now.AddDays(-60);

        var currentCompletions = await _context.Enrollments
            .CountAsync(e => e.CompletedAt.HasValue && e.CompletedAt >= last30Days);
        
        var previousCompletions = await _context.Enrollments
            .CountAsync(e => e.CompletedAt.HasValue && e.CompletedAt >= previous30Days && e.CompletedAt < last30Days);

        var activeLearnersCount = await _context.Enrollments
            .Where(e => e.EnrolledAt >= last30Days || (e.Progress > 0 && e.Progress < 100 && e.EnrolledAt >= previous30Days))
            .Select(e => e.UserId)
            .Distinct()
            .CountAsync();

        var previousActiveCount = await _context.Enrollments
            .Where(e => e.EnrolledAt >= previous30Days && e.EnrolledAt < last30Days)
            .Select(e => e.UserId)
            .Distinct()
            .CountAsync();

        var avgCompletionDays = await _context.Enrollments
            .Where(e => e.CompletedAt.HasValue && e.CompletedAt >= last30Days)
            .Select(e => EF.Functions.DateDiffDay(e.EnrolledAt, e.CompletedAt.Value))
            .AverageAsync(d => (double?)d) ?? 0;

        var previousAvgDays = await _context.Enrollments
            .Where(e => e.CompletedAt.HasValue && e.CompletedAt >= previous30Days && e.CompletedAt < last30Days)
            .Select(e => EF.Functions.DateDiffDay(e.EnrolledAt, e.CompletedAt.Value))
            .AverageAsync(d => (double?)d) ?? 0;

        var avgScore = await _context.QuizAttempts
            .Where(q => q.AttemptedAt >= last30Days)
            .AverageAsync(q => (double?)q.Score) ?? 0;

        var previousAvgScore = await _context.QuizAttempts
            .Where(q => q.AttemptedAt >= previous30Days && q.AttemptedAt < last30Days)
            .AverageAsync(q => (double?)q.Score) ?? 0;

        return Ok(new
        {
            courseCompletions = currentCompletions,
            courseCompletionsChange = previousCompletions > 0 ? Math.Round((double)(currentCompletions - previousCompletions) / previousCompletions * 100, 0) : 0,
            activeLearnersCount,
            activeLearnersChange = previousActiveCount > 0 ? Math.Round((double)(activeLearnersCount - previousActiveCount) / previousActiveCount * 100, 0) : 0,
            avgCompletionWeeks = avgCompletionDays > 0 ? Math.Round(avgCompletionDays / 7, 1) : 0,
            avgCompletionChange = previousAvgDays > 0 ? Math.Round((previousAvgDays - avgCompletionDays) / previousAvgDays * 100, 0) : 0,
            satisfactionScore = Math.Round(avgScore / 20, 1),
            satisfactionChange = previousAvgScore > 0 ? Math.Round((avgScore - previousAvgScore) / previousAvgScore * 100, 0) : 0
        });
    }

    [HttpGet("department-performance")]
    public async Task<IActionResult> GetDepartmentPerformance()
    {
        var departments = await _context.Users
            .Where(u => !string.IsNullOrEmpty(u.Department))
            .GroupBy(u => u.Department)
            .Select(g => new
            {
                department = g.Key,
                totalUsers = g.Count(),
                activeLearnersCount = g.Count(u => _context.Enrollments.Any(e => e.UserId == u.Id && e.Progress > 0 && e.Progress < 100)),
                completions = _context.Enrollments.Count(e => g.Select(u => u.Id).Contains(e.UserId) && e.CompletedAt.HasValue),
                avgScore = _context.QuizAttempts
                    .Where(q => g.Select(u => u.Id).Contains(q.UserId))
                    .Average(q => (double?)q.Score) ?? 0,
                progress = _context.Enrollments
                    .Where(e => g.Select(u => u.Id).Contains(e.UserId))
                    .Average(e => (double?)e.Progress) ?? 0
            })
            .OrderByDescending(d => d.completions)
            .ToListAsync();

        return Ok(departments);
    }

    [HttpGet("top-learners")]
    public async Task<IActionResult> GetTopLearners()
    {
        var topLearners = await _context.Users
            .Select(u => new
            {
                u.Id,
                u.Name,
                department = u.Department ?? "N/A",
                completions = _context.Enrollments.Count(e => e.UserId == u.Id && e.CompletedAt.HasValue)
            })
            .OrderByDescending(u => u.completions)
            .Take(10)
            .ToListAsync();

        return Ok(topLearners);
    }

    [HttpGet("popular-courses")]
    public async Task<IActionResult> GetPopularCourses()
    {
        var popularCourses = await _context.Courses
            .Select(c => new
            {
                c.Id,
                c.Title,
                enrollments = _context.Enrollments.Count(e => e.CourseId == c.Id)
            })
            .OrderByDescending(c => c.enrollments)
            .Take(10)
            .ToListAsync();

        return Ok(popularCourses);
    }

    [HttpGet("progress-over-time")]
    public async Task<IActionResult> GetProgressOverTime()
    {
        var now = DateTime.UtcNow.AddHours(5).AddMinutes(30);
        var last30Days = now.AddDays(-30);

        var progressData = await _context.Enrollments
            .Where(e => e.EnrolledAt >= last30Days)
            .GroupBy(e => e.EnrolledAt.Date)
            .Select(g => new
            {
                date = g.Key,
                enrollments = g.Count(),
                completions = g.Count(e => e.CompletedAt.HasValue)
            })
            .OrderBy(d => d.date)
            .ToListAsync();

        return Ok(progressData);
    }

    [HttpGet("manager/{managerId}")]
    public async Task<IActionResult> GetManagerReports(int managerId)
    {
        var manager = await _context.Users.FindAsync(managerId);
        if (manager == null) return NotFound();

        var teamMembers = await _context.Users
            .Where(u => u.Department == manager.Department && u.Role == "Employee")
            .Select(u => u.Id)
            .ToListAsync();

        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.User)
            .Where(e => teamMembers.Contains(e.UserId))
            .ToListAsync();

        var userPoints = await _context.UserPoints
            .Where(up => teamMembers.Contains(up.UserId))
            .GroupBy(up => up.UserId)
            .Select(g => new { UserId = g.Key, TotalPoints = g.Sum(up => up.TotalPoints) })
            .ToDictionaryAsync(x => x.UserId, x => x.TotalPoints);

        var users = await _context.Users
            .Where(u => teamMembers.Contains(u.Id))
            .ToListAsync();

        var certificates = await _context.Certificates
            .Where(c => teamMembers.Contains(c.UserId))
            .GroupBy(c => c.UserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        var employees = users.Select(u => new
        {
            id = u.Id,
            name = u.FullName,
            email = u.Email,
            department = u.Department,
            totalPoints = userPoints.ContainsKey(u.Id) ? userPoints[u.Id] : 0,
            completedCourses = enrollments.Count(e => e.UserId == u.Id && e.CompletedAt != null),
            inProgressCourses = enrollments.Count(e => e.UserId == u.Id && e.CompletedAt == null && e.Progress > 0),
            certificates = certificates.ContainsKey(u.Id) ? certificates[u.Id] : 0,
            avgProgress = enrollments.Where(e => e.UserId == u.Id).Any() ? enrollments.Where(e => e.UserId == u.Id).Average(e => e.Progress) : 0
        }).ToList();

        var courseProgress = enrollments
            .GroupBy(e => e.Course.Title)
            .Select(g => new
            {
                course = g.Key,
                enrolled = g.Count(),
                completed = g.Count(e => e.CompletedAt != null),
                avgProgress = g.Average(e => e.Progress)
            })
            .OrderByDescending(c => c.enrolled)
            .Take(10)
            .ToList();

        var progressOverTime = enrollments
            .Where(e => e.CompletedAt != null)
            .GroupBy(e => new { e.CompletedAt.Value.Year, e.CompletedAt.Value.Month })
            .Select(g => new
            {
                month = $"{g.Key.Year}-{g.Key.Month:D2}",
                completions = g.Count()
            })
            .OrderBy(x => x.month)
            .ToList();

        var departmentStats = enrollments
            .GroupBy(e => e.User.Department)
            .Select(g => new
            {
                department = g.Key ?? "N/A",
                employees = g.Select(e => e.UserId).Distinct().Count(),
                completions = g.Count(e => e.CompletedAt != null),
                avgProgress = g.Average(e => e.Progress)
            })
            .ToList();

        return Ok(new
        {
            employees,
            courseProgress,
            progressOverTime,
            departmentStats,
            summary = new
            {
                totalEmployees = teamMembers.Count,
                totalEnrollments = enrollments.Count,
                totalCompletions = enrollments.Count(e => e.CompletedAt != null),
                avgCompletionRate = enrollments.Any() ? Math.Round((double)enrollments.Count(e => e.CompletedAt != null) / enrollments.Count * 100, 1) : 0,
                totalPoints = userPoints.Values.Sum()
            }
        });
    }
}
