namespace learning_path_tracker.Application.DTOs.Courses;

public class CourseStatisticsDto
{
    public int TotalEnrollments { get; set; }
    public int CompletedEnrollments { get; set; }
    public int ActiveEnrollments { get; set; }
    public double AverageProgress { get; set; }
}
