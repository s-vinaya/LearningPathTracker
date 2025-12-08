namespace learning_path_tracker.Domain.Entities;

public class Approval
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int ManagerId { get; set; }
    public string Type { get; set; } = string.Empty; // LPRequest, ExtensionRequest, ResourceRequest, CertificateValidation, CourseRequest
    public string Payload { get; set; } = string.Empty; // JSON details
    public string? RequestDetails { get; set; } // Additional request information
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public string? ReviewerComments { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    
    public User Employee { get; set; } = null!;
    public User? Manager { get; set; }
}
