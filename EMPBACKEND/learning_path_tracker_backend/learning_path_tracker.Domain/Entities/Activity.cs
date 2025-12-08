namespace learning_path_tracker.Domain.Entities;

public class Activity
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
