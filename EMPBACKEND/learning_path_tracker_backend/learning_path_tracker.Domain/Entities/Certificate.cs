namespace learning_path_tracker.Domain.Entities;

public class Certificate
{
    public int Id { get; set; }
    public string CertificateId { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int? CourseId { get; set; }
    public string? CourseName { get; set; }
    public int? LearningPathId { get; set; }
    public string? LearningPathName { get; set; }
    public string? ManagerName { get; set; }
    public double AverageScore { get; set; }
    public DateTime IssuedAt { get; set; }
    public string CertificateType { get; set; } = "Course"; // "Course" or "LearningPath"
    public User User { get; set; } = null!;
    public Course? Course { get; set; }
    public LearningPath? LearningPath { get; set; }
}
