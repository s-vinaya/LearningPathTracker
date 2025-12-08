namespace learning_path_tracker.Domain.Entities;

public class EmailLog
{
    public int Id { get; set; }
    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime SentAt { get; set; }
}
