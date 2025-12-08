namespace learning_path_tracker.Application.DTOs;

public class TeamSettingsDto
{
    public int DefaultLearningHours { get; set; }
    public int AssignmentDeadline { get; set; }
    public string ReminderFrequency { get; set; } = "Weekly";
    public string AutoAssignNewPaths { get; set; } = "Disabled";
}
