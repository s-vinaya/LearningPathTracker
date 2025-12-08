namespace learning_path_tracker.Application.DTOs;

public class ApprovalDecisionDto
{
    public int ApprovalId { get; set; }
    public string Status { get; set; } = string.Empty; // Approved or Rejected
    public string ReviewerComments { get; set; } = string.Empty;
}
