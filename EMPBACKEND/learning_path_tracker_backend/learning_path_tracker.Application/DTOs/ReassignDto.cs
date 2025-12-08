namespace learning_path_tracker.Application.DTOs;

public class ReassignDto
{
    public int AssignmentId { get; set; }
    public int NewLearningPathId { get; set; }
    public DateTime? NewDueDate { get; set; }
    public string? ManagerNotes { get; set; }
}
