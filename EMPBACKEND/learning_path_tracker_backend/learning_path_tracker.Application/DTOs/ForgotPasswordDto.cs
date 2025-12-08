namespace learning_path_tracker.Application.DTOs;

public class ForgotPasswordDto
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class CheckEmailDto
{
    public string Email { get; set; } = string.Empty;
}
