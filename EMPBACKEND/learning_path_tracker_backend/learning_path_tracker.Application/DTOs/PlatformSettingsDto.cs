namespace learning_path_tracker.Application.DTOs;

public class PlatformSettingsDto
{
    public string PlatformName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string PlatformDescription { get; set; } = string.Empty;
    public bool AutoEnroll { get; set; }
    public bool Certificates { get; set; }
}
