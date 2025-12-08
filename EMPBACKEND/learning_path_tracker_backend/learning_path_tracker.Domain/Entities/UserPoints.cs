namespace learning_path_tracker.Domain.Entities;

public class UserPoints
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public int CourseCompletionPoints { get; set; }
    public int QuizPoints { get; set; }
    public int TotalPoints { get; set; }
    public DateTime EarnedAt { get; set; }
    public User User { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
