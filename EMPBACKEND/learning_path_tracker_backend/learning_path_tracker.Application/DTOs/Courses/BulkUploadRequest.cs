namespace learning_path_tracker.Application.DTOs.Courses;

public class BulkUploadRequest
{
    public List<string> YouTubeUrls { get; set; } = new();
}
