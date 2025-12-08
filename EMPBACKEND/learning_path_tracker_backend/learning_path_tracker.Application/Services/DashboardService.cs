using learning_path_tracker.Application.DTOs.Dashboard;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Data.Repositories;
using learning_path_tracker.Database;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICertificateRepository _certificateRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IActivityRepository _activityRepository;
    private readonly AppDbContext _context;

    public DashboardService(
        IUserRepository userRepository,
        ICourseRepository courseRepository,
        ICertificateRepository certificateRepository,
        IEnrollmentRepository enrollmentRepository,
        IActivityRepository activityRepository,
        AppDbContext context)
    {
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _certificateRepository = certificateRepository;
        _enrollmentRepository = enrollmentRepository;
        _activityRepository = activityRepository;
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        var totalEmails = await _context.EmailLogs.CountAsync();
        var successfulEmails = await _context.EmailLogs.CountAsync(e => e.IsSuccess);
        var deliveryRate = totalEmails > 0 ? (double)successfulEmails / totalEmails * 100 : 0;

        return new DashboardSummaryDto
        {
            TotalUsers = await _userRepository.GetTotalUserCountAsync(),
            ActiveCourses = await _courseRepository.GetActiveCourseCountAsync(),
            CoursesCompleted = await _courseRepository.GetCompletedCourseCountAsync(),
            CertificatesIssued = await _certificateRepository.GetTotalCertificatesIssuedAsync(),
            EmailDeliveryRate = Math.Round(deliveryRate, 2)
        };
    }

    public async Task<EngagementDataDto> GetEngagementDataAsync(int days)
    {
        var endDate = DateTime.UtcNow.Date.AddDays(1);
        var startDate = endDate.AddDays(-days);
        
        var completions = new List<int>();
        var registrations = new List<int>();
        var labels = new List<string>();

        for (int i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            var nextDate = date.AddDays(1);

            var dayCompletions = await _enrollmentRepository.GetCompletionsByDateRangeAsync(date, nextDate);
            var dayRegistrations = await _userRepository.GetUsersByDateRangeAsync(date, nextDate);

            completions.Add(dayCompletions.Count);
            registrations.Add(dayRegistrations.Count);
            labels.Add(date.ToString("MMM dd"));
        }

        return new EngagementDataDto
        {
            CourseCompletions = completions,
            Registrations = registrations,
            Labels = labels
        };
    }

    public async Task<List<RecentActivityDto>> GetRecentActivitiesAsync()
    {
        var activities = new List<RecentActivityDto>();
        
        var recentUsers = await _userRepository.GetUsersByDateRangeAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(1));
        foreach (var user in recentUsers.OrderByDescending(u => u.CreatedAt).Take(5))
        {
            activities.Add(new RecentActivityDto
            {
                Type = "user",
                Text = $"{user.Name} registered to the platform",
                TimeAgo = GetTimeAgo(user.CreatedAt)
            });
        }
        
        var recentCompletions = await _enrollmentRepository.GetCompletionsByDateRangeAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(1));
        foreach (var enrollment in recentCompletions.OrderByDescending(e => e.CompletedAt).Take(5))
        {
            activities.Add(new RecentActivityDto
            {
                Type = "course",
                Text = $"Course completed",
                TimeAgo = GetTimeAgo(enrollment.CompletedAt ?? enrollment.EnrolledAt)
            });
        }
        
        return activities.OrderByDescending(a => a.TimeAgo).Take(10).ToList();
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;
        
        if (timeSpan.TotalMinutes < 1) return "just now";
        if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minutes ago";
        if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hours ago";
        if (timeSpan.TotalDays < 30) return $"{(int)timeSpan.TotalDays} days ago";
        
        return dateTime.ToString("MMM dd, yyyy");
    }
}
