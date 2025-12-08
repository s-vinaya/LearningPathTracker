namespace learning_path_tracker.Application.DTOs.LearningPaths;

public class UpdateLearningPathDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
