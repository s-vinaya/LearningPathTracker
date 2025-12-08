namespace learning_path_tracker.Application.DTOs;

public class UploadProfileImageDTO
{
    public int UserId { get; set; }
    public string ImageBase64 { get; set; } = string.Empty;
}
