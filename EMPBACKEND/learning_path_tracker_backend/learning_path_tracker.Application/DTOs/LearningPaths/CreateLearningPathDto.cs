namespace learning_path_tracker.Application.DTOs.LearningPaths;

public class CreateLearningPathDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Level { get; set; } = "Beginner";
    public bool IsActive { get; set; } = true;
    public List<CreateModuleDto> Modules { get; set; } = new();
}
