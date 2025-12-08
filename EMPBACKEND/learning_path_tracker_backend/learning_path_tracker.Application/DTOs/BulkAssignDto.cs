namespace learning_path_tracker.Application.DTOs;

public class BulkAssignDto
{
    public int LearningPathId { get; set; }
    public DateTime? DueDate { get; set; }
    public string? ManagerNotes { get; set; }
}
