namespace learning_path_tracker.Application.DTOs.Notifications;

public class CreateNotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Recipients { get; set; } = "all";
}
