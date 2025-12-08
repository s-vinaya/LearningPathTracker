namespace learning_path_tracker.Application.DTOs;

public class CertificateDto
{
    public int Id { get; set; }
    public string CertificateId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string? CourseName { get; set; }
    public string? LearningPathName { get; set; }
    public string? ManagerName { get; set; }
    public double AverageScore { get; set; }
    public DateTime IssuedAt { get; set; }
    public string CertificateType { get; set; } = string.Empty;
}
