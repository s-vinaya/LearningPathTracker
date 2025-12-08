namespace learning_path_tracker.Application.DTOs.Users;

public class UserDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsApproved { get; set; }
    public string? ProfileImageBase64 { get; set; }
    public List<UserEnrollmentDto> Enrollments { get; set; } = new();
}

public class UserEnrollmentDto
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int Progress { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
