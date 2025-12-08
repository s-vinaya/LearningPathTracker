namespace learning_path_tracker.Application.DTOs;

public class LearningScheduleDto
{
    public int Id { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class SetLearningScheduleDto
{
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}
