namespace learning_path_tracker.Domain.Entities;

public class Module
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public int LearningPathId { get; set; }
    public LearningPath LearningPath { get; set; } = null!;
    public int? CourseId { get; set; }
    public Course? Course { get; set; }
}
