namespace learning_path_tracker.Application.DTOs.LearningPaths;

public class LearningPathDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int EnrollmentCount { get; set; }
    public List<ModuleDto> Modules { get; set; } = new();
}
