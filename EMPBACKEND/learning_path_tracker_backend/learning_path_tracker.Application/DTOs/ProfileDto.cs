namespace learning_path_tracker.Application.DTOs;

public class ProfileDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? ProfileImageBase64 { get; set; }
    public DateTime? LastLogin { get; set; }
}

public class UpdateProfileDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public string? Team { get; set; }
}
