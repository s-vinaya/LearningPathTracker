namespace learning_path_tracker.Domain.Entities;

public class LearningSchedule
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public User User { get; set; } = null!;
}
