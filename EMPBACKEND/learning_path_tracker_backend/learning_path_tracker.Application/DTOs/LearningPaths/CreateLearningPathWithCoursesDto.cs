namespace learning_path_tracker.Application.DTOs.LearningPaths;

public class CreateLearningPathWithCoursesDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EstimatedHours { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<int> CourseIds { get; set; } = new();
}