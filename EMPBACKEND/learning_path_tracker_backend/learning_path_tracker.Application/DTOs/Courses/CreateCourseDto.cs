namespace learning_path_tracker.Application.DTOs.Courses;

public class CreateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public int DurationHours { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string? YouTubeVideoId { get; set; }
    public string? YouTubeUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? VideoDuration { get; set; }
}
