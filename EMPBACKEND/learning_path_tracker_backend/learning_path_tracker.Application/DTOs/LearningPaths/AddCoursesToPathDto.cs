namespace learning_path_tracker.Application.DTOs.LearningPaths;

public class AddCoursesToPathDto
{
    public int LearningPathId { get; set; }
    public List<int> CourseIds { get; set; } = new();
}