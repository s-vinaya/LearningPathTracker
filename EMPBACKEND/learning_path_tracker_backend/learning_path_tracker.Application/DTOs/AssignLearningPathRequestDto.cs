namespace learning_path_tracker.Application.DTOs;

public class AssignLearningPathRequestDto
{
    public int LearningPathId { get; set; }
    public List<int> EmployeeIds { get; set; } = new();
    public DateTime? DueDate { get; set; }
    public string? ManagerNotes { get; set; }
}
