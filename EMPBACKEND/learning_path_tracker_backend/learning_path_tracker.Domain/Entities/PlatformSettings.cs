namespace learning_path_tracker.Domain.Entities;

public class PlatformSettings
{
    public int Id { get; set; }
    public string PlatformName { get; set; } = "LearnTrack";
    public string CompanyName { get; set; } = "Your Company";
    public string PlatformDescription { get; set; } = "A comprehensive learning management system for employee development";
    public bool AutoEnroll { get; set; } = true;
    public bool Certificates { get; set; } = true;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
