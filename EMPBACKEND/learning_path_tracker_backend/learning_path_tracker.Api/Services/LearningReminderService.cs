using learning_path_tracker.Application.Constants;
using learning_path_tracker.Application.Services;
using learning_path_tracker.Database;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Api.Services;

public class LearningReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public LearningReminderService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAndSendReminders();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task CheckAndSendReminders()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

            var currentTime = DateTime.Now.TimeOfDay;
            var currentHour = currentTime.Hours;
            var currentMinute = currentTime.Minutes;

            var schedules = await context.LearningSchedules
                .Include(s => s.User)
                .Where(s => s.IsActive && 
                            s.StartTime.Hours == currentHour && 
                            s.StartTime.Minutes == currentMinute)
                .ToListAsync();

            foreach (var schedule in schedules)
            {
                await emailService.SendEmailAsync(
                    schedule.User.Email,
                    EmailConstants.LearningReminderSubject,
                    EmailConstants.GetLearningReminderBody(schedule.User.FullName)
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in LearningReminderService: {ex.Message}");
        }
    }
}
