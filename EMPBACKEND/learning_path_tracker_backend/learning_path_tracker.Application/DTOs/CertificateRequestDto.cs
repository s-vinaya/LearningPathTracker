namespace learning_path_tracker.Application.DTOs;

public class CertificateRequestDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int? CourseId { get; set; }
    public string? CourseName { get; set; }
    public int? LearningPathId { get; set; }
    public string? LearningPathName { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public DateTime CompletedDate { get; set; }
    public DateTime RequestedDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ApproveCertificateRequestDto
{
    public int RequestId { get; set; }
    public int Id { get; set; }
    public bool Approve { get; set; }
    public bool Approved { get; set; }
    public string? RejectionReason { get; set; }
    public string? Reason { get; set; }
}
