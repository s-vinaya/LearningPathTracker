namespace learning_path_tracker.Application.DTOs.LearningPaths;

public class UpdateCourseOrderDto
{
    public int LearningPathId { get; set; }
    public List<CourseOrderDto> CourseOrders { get; set; } = new();
}

public class CourseOrderDto
{
    public int CourseId { get; set; }
    public int Order { get; set; }
}