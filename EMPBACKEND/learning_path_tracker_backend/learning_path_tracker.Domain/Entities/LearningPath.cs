namespace learning_path_tracker.Domain.Entities;

public class LearningPath
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Level { get; set; } = "Beginner";
    public string DifficultyLevel => Level;
    public int EstimatedHours { get; set; } = 10;
    public int EstimatedDuration => EstimatedHours;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public DateTime CreatedAt => CreatedOn;
    public bool IsActive { get; set; }
    public ICollection<Module> Modules { get; set; } = new List<Module>();
    public ICollection<LearningPathCourse> LearningPathCourses { get; set; } = new List<LearningPathCourse>();
}
