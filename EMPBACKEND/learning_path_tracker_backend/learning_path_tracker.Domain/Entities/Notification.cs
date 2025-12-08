namespace learning_path_tracker.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public string Recipients { get; set; } = "all";
    public int RecipientCount { get; set; }
    public int DeliveredCount { get; set; }
    public int OpenedCount { get; set; }
    public int ClickedCount { get; set; }
    public ICollection<NotificationTracking> Trackings { get; set; } = new List<NotificationTracking>();
}

public class NotificationTracking
{
    public int Id { get; set; }
    public int NotificationId { get; set; }
    public int UserId { get; set; }
    public bool IsDelivered { get; set; }
    public bool IsOpened { get; set; }
    public bool IsClicked { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? OpenedAt { get; set; }
    public DateTime? ClickedAt { get; set; }
    public Notification Notification { get; set; } = null!;
    public User User { get; set; } = null!;
}
