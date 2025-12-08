namespace learning_path_tracker.Application.DTOs;

public class NotificationSettingsDto
{
    public bool EmailNotifications { get; set; }
    public bool WeeklyProgressReports { get; set; }
    public bool CourseCompletionAlerts { get; set; }
    public bool AssignmentReminders { get; set; }
    public bool TeamUpdates { get; set; }
    public bool SystemNotifications { get; set; }
}
