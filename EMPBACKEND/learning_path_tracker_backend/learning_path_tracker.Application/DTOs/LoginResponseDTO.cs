namespace learning_path_tracker.Application.DTOs;

public class LoginResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public UserResponseDTO User { get; set; } = null!;
}
