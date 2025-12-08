namespace learning_path_tracker.Application.DTOs;

public class ApprovalHistoryDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string? ReviewerComments { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
