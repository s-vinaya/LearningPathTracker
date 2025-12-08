namespace learning_path_tracker.Domain.Entities;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public User? Manager { get; set; }
}
