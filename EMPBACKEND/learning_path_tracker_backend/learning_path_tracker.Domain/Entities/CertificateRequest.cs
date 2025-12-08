namespace learning_path_tracker.Domain.Entities;

public class CertificateRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int? CourseId { get; set; }
    public int? LearningPathId { get; set; }
    public string RequestType { get; set; } = "Course"; // "Course" or "LearningPath"
    public DateTime CompletedDate { get; set; }
    public DateTime RequestedDate { get; set; }
    public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"
    public int? ReviewedByManagerId { get; set; }
    public DateTime? ReviewedDate { get; set; }
    public string? RejectionReason { get; set; }
    public int? CertificateId { get; set; }
    public User Employee { get; set; } = null!;
    public Course? Course { get; set; }
    public LearningPath? LearningPath { get; set; }
    public User? ReviewedByManager { get; set; }
    public Certificate? Certificate { get; set; }
}
