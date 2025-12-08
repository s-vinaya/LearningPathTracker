namespace learning_path_tracker.Domain.Entities;

public class Assignment
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int LearningPathId { get; set; }
    public int AssignedByManagerId { get; set; }
    public DateTime AssignedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? ManagerNotes { get; set; }
    public int ProgressPercent { get; set; }
    public string Status { get; set; } = "Assigned"; // Assigned, InProgress, Completed, Overdue
    public DateTime? CompletedDate { get; set; }
    
    public User Employee { get; set; } = null!;
    public LearningPath LearningPath { get; set; } = null!;
    public User AssignedByManager { get; set; } = null!;
}
