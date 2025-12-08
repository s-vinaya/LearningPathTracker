namespace learning_path_tracker.Application.DTOs.Dashboard;

public class EngagementDataDto
{
    public List<int> CourseCompletions { get; set; } = new();
    public List<int> Registrations { get; set; } = new();
    public List<string> Labels { get; set; } = new();
}
