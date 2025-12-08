namespace learning_path_tracker.Domain.Entities;

public class LearningPathCourse
{
    public int LearningPathId { get; set; }
    public int CourseId { get; set; }
    public int Order { get; set; }
    public int SortOrder => Order;
    public LearningPath LearningPath { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
