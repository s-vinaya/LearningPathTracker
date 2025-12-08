namespace learning_path_tracker.Application.DTOs.LearningPaths;

public class ModuleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public int LearningPathId { get; set; }
}
