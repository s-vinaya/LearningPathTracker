using learning_path_tracker.Application.Services;
using learning_path_tracker.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PointsCalculationService _pointsService;

    public LeaderboardController(AppDbContext context, PointsCalculationService pointsService)
    {
        _context = context;
        _pointsService = pointsService;
    }

    [HttpGet("employee/{userId}")]
    public async Task<IActionResult> GetEmployeeLeaderboard(int userId)
    {
        var userPointsDict = await _pointsService.GetAllUsersTotalPoints();

        var allUsers = await _context.Users
            .Where(u => u.Role == "Employee")
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.ProfileImage,
                completedCourses = _context.Enrollments.Count(e => e.UserId == u.Id && e.CompletedAt.HasValue),
                certificatesEarned = _context.Certificates.Count(c => c.UserId == u.Id),
                totalPoints = userPointsDict.ContainsKey(u.Id) ? userPointsDict[u.Id] : 0
            })
            .ToListAsync();

        var rankedUsers = allUsers
            .OrderByDescending(u => u.totalPoints)
            .ThenByDescending(u => u.completedCourses)
            .ThenByDescending(u => u.certificatesEarned)
            .Select((u, index) => new
            {
                rank = index + 1,
                u.Id,
                u.FullName,
                u.Email,
                profileImage = u.ProfileImage != null ? Convert.ToBase64String(u.ProfileImage) : null,
                u.completedCourses,
                u.certificatesEarned,
                u.totalPoints
            }).ToList();

        var top3 = rankedUsers.Take(3).ToList();
        var currentUser = rankedUsers.FirstOrDefault(u => u.Id == userId);

        if (currentUser != null && currentUser.rank > 3)
        {
            return Ok(new { top3, currentUser });
        }

        return Ok(new { top3, currentUser = (object?)null });
    }

    [HttpGet("manager/{managerId}")]
    public async Task<IActionResult> GetManagerTeamLeaderboard(int managerId)
    {
        var manager = await _context.Users.FindAsync(managerId);
        if (manager == null) return NotFound();

        var userPointsDict = await _pointsService.GetAllUsersTotalPoints();

        var teamMembers = await _context.Users
            .Where(u => u.Department == manager.Department && u.Role == "Employee")
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.ProfileImage,
                completedCourses = _context.Enrollments.Count(e => e.UserId == u.Id && e.CompletedAt.HasValue),
                certificatesEarned = _context.Certificates.Count(c => c.UserId == u.Id),
                totalPoints = userPointsDict.ContainsKey(u.Id) ? userPointsDict[u.Id] : 0
            })
            .ToListAsync();

        var leaderboard = teamMembers
            .OrderByDescending(u => u.totalPoints)
            .ThenByDescending(u => u.completedCourses)
            .ThenByDescending(u => u.certificatesEarned)
            .Select((u, index) => new
            {
                rank = index + 1,
                u.Id,
                u.FullName,
                u.Email,
                profileImage = u.ProfileImage != null ? Convert.ToBase64String(u.ProfileImage) : null,
                u.completedCourses,
                u.certificatesEarned,
                u.totalPoints
            }).ToList();

        return Ok(leaderboard);
    }

    [HttpGet("streaks/{userId}")]
    public async Task<IActionResult> GetUserStreaks(int userId)
    {
        var activities = await _context.Activities
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(365)
            .ToListAsync();

        var streakData = activities
            .GroupBy(a => a.CreatedAt.Date)
            .Select(g => new { date = g.Key, count = g.Count() })
            .OrderBy(x => x.date)
            .ToList();

        var currentStreak = CalculateCurrentStreak(streakData.Select(s => s.date).ToList());
        var longestStreak = CalculateLongestStreak(streakData.Select(s => s.date).ToList());
        var totalSubmissions = activities.Count;

        return Ok(new
        {
            currentStreak,
            longestStreak,
            totalSubmissions,
            streakData = streakData.Select(s => new { s.date, s.count })
        });
    }

    private int CalculateCurrentStreak(List<DateTime> dates)
    {
        if (!dates.Any()) return 0;
        
        var today = DateTime.UtcNow.Date;
        var streak = 0;
        
        for (int i = 0; i <= 365; i++)
        {
            if (dates.Contains(today.AddDays(-i)))
                streak++;
            else
                break;
        }
        
        return streak;
    }

    private int CalculateLongestStreak(List<DateTime> dates)
    {
        if (!dates.Any()) return 0;
        
        var sortedDates = dates.OrderBy(d => d).ToList();
        int maxStreak = 1;
        int currentStreak = 1;
        
        for (int i = 1; i < sortedDates.Count; i++)
        {
            if ((sortedDates[i] - sortedDates[i - 1]).Days == 1)
            {
                currentStreak++;
                maxStreak = Math.Max(maxStreak, currentStreak);
            }
            else
            {
                currentStreak = 1;
            }
        }
        
        return maxStreak;
    }

    [HttpPost("recalculate-points")]
    public async Task<IActionResult> RecalculatePoints()
    {
        await _pointsService.RecalculatePointsForExistingCompletions();
        return Ok(new { message = "Points recalculated successfully for all completed courses" });
    }

    [HttpGet("debug/points-status")]
    public async Task<IActionResult> GetPointsDebugStatus()
    {
        var completedEnrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.User)
            .Where(e => e.CompletedAt != null)
            .Select(e => new
            {
                userId = e.UserId,
                userName = e.User.FullName,
                courseId = e.CourseId,
                courseName = e.Course.Title,
                completedAt = e.CompletedAt,
                quizPassed = e.QuizPassed,
                hasPoints = _context.UserPoints.Any(up => up.UserId == e.UserId && up.CourseId == e.CourseId)
            })
            .ToListAsync();

        var totalCompleted = completedEnrollments.Count;
        var withPoints = completedEnrollments.Count(e => e.hasPoints);
        var withoutPoints = totalCompleted - withPoints;

        return Ok(new
        {
            summary = new { totalCompleted, withPoints, withoutPoints },
            details = completedEnrollments
        });
    }
}
