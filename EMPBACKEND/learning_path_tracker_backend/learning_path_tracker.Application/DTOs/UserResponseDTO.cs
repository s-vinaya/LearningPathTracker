namespace learning_path_tracker.Application.DTOs;

public class UserResponseDTO
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string ProfileImageBase64 { get; set; } = string.Empty;
    public DateTime LastLogin { get; set; }
}
