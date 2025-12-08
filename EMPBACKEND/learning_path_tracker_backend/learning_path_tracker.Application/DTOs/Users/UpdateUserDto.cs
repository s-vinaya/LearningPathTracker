namespace learning_path_tracker.Application.DTOs.Users;

public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Department { get; set; }
    public int? ManagerId { get; set; }
}
