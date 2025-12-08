using learning_path_tracker.Application.DTOs.Courses;
using CourseStats = learning_path_tracker.Application.DTOs.Courses.CourseStatisticsDto;
using AllCourseStats = learning_path_tracker.Application.DTOs.Statistics.CourseStatisticsDto;

namespace learning_path_tracker.Application.Interfaces;

public interface ICourseService
{
    Task<List<CourseDto>> GetAllCoursesAsync();
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto);
    Task<CourseDto?> UpdateCourseAsync(int id, UpdateCourseDto dto);
    Task<object> CheckCourseDeleteImpactAsync(int id);
    Task<bool> DeleteCourseAsync(int id);
    Task<CourseStats?> GetCourseStatisticsAsync(int id);
    Task<AllCourseStats> GetAllCourseStatisticsAsync();
    Task<BulkUploadResult> BulkUploadCoursesAsync(List<string> youTubeUrls);
}

//TODO: move this to correct path
public class BulkUploadResult
{
    public int TotalProcessed { get; set; }
    public int SuccessCount { get; set; }
    public int SkippedCount { get; set; }
    public List<string> SkippedUrls { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}
