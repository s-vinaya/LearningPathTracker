namespace learning_path_tracker.Domain.Entities;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public int DurationHours { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? YouTubeVideoId { get; set; }
    public string? YouTubeUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? VideoDuration { get; set; }
    public int? QuizId { get; set; }
    public bool HasQuiz => QuizId.HasValue;
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<LearningPathCourse> LearningPathCourses { get; set; } = new List<LearningPathCourse>();
}
