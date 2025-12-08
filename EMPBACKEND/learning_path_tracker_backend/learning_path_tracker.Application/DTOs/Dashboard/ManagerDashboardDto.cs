namespace learning_path_tracker.Application.DTOs.Dashboard;

public class ManagerDashboardDto
{
    public int TotalEmployees { get; set; }
    public int ActiveLearningPaths { get; set; }
    public int CompletedLearningPaths { get; set; }
    public int PendingApprovals { get; set; }
    public List<UpcomingDeadlineDto> UpcomingDeadlines { get; set; } = new();
}

public class UpcomingDeadlineDto
{
    public int AssignmentId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? EmployeeEmail { get; set; }
    public string? EmployeeDepartment { get; set; }
    public string LearningPathTitle { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime AssignedDate { get; set; }
    public int ProgressPercent { get; set; }
    public string Status { get; set; } = string.Empty;
    public int DaysRemaining { get; set; }
}
