using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Application.Services;

public class PointsCalculationService
{
    private readonly AppDbContext _context;

    public PointsCalculationService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Calculate points based on course duration
    /// Duration < 5 min = 10 points
    /// Duration 5-10 min = 20 points
    /// Duration 10-30 min = 50 points
    /// Duration 30-60 min = 100 points
    /// Duration > 60 min = 150 points
    /// </summary>
    public int CalculateCourseCompletionPoints(double durationHours, string videoDuration = null)
    {
        double durationMinutes = 0;
        
        if (!string.IsNullOrEmpty(videoDuration))
        {
            var parts = videoDuration.Split(':');
            if (parts.Length >= 2 && int.TryParse(parts[0], out int mins))
            {
                durationMinutes = mins;
            }
        }
        else
        {
            durationMinutes = durationHours * 60;
        }

        if (durationMinutes < 5) return 10;
        if (durationMinutes < 10) return 20;
        if (durationMinutes < 30) return 50;
        if (durationMinutes < 60) return 100;
        return 150;
    }

    /// <summary>
    /// Calculate quiz points based on score percentage
    /// Score < 50% = 0 points
    /// Score 50-70% = 10 points
    /// Score 70-85% = 20 points
    /// Score 85-95% = 30 points
    /// Score >= 95% = 50 points
    /// </summary>
    public int CalculateQuizPoints(double scorePercentage)
    {
        if (scorePercentage < 50) return 0;
        if (scorePercentage < 70) return 10;
        if (scorePercentage < 85) return 20;
        if (scorePercentage < 95) return 30;
        return 50;
    }

    public async Task AwardPointsForCourseCompletion(int userId, int courseId)
    {
        var existingPoints = await _context.UserPoints
            .FirstOrDefaultAsync(up => up.UserId == userId && up.CourseId == courseId);

        var course = await _context.Courses.FindAsync(courseId);
        if (course == null) return;

        var coursePoints = CalculateCourseCompletionPoints(course.DurationHours, course.VideoDuration);

        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        var quizPoints = 0;
        if (course.QuizId.HasValue && enrollment != null && enrollment.QuizPassed)
        {
            var quizAttempts = await _context.QuizAttempts
                .Where(qa => qa.UserId == userId && qa.QuizId == course.QuizId.Value && qa.Passed)
                .OrderByDescending(qa => qa.Score)
                .FirstOrDefaultAsync();

            if (quizAttempts != null)
            {
                quizPoints = CalculateQuizPoints(quizAttempts.Score);
            }
        }

        if (existingPoints != null)
        {
            existingPoints.CourseCompletionPoints = coursePoints;
            existingPoints.QuizPoints = quizPoints;
            existingPoints.TotalPoints = coursePoints + quizPoints;
            _context.UserPoints.Update(existingPoints);
        }
        else
        {
            var userPoints = new UserPoints
            {
                UserId = userId,
                CourseId = courseId,
                CourseCompletionPoints = coursePoints,
                QuizPoints = quizPoints,
                TotalPoints = coursePoints + quizPoints,
                EarnedAt = DateTime.UtcNow
            };
            _context.UserPoints.Add(userPoints);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetUserTotalPoints(int userId)
    {
        return await _context.UserPoints
            .Where(up => up.UserId == userId)
            .SumAsync(up => up.TotalPoints);
    }

    public async Task<Dictionary<int, int>> GetAllUsersTotalPoints()
    {
        return await _context.UserPoints
            .GroupBy(up => up.UserId)
            .Select(g => new { UserId = g.Key, TotalPoints = g.Sum(up => up.TotalPoints) })
            .ToDictionaryAsync(x => x.UserId, x => x.TotalPoints);
    }

    public async Task RecalculatePointsForExistingCompletions()
    {
        var completedEnrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.CompletedAt != null)
            .ToListAsync();

        var processedCount = 0;
        foreach (var enrollment in completedEnrollments)
        {
            var existingPoints = await _context.UserPoints
                .FirstOrDefaultAsync(up => up.UserId == enrollment.UserId && up.CourseId == enrollment.CourseId);
            
            if (existingPoints == null)
            {
                await AwardPointsForCourseCompletion(enrollment.UserId, enrollment.CourseId);
                processedCount++;
            }
        }
    }
}
