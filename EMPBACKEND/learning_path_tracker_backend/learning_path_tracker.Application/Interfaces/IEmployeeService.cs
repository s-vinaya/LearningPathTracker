namespace learning_path_tracker.Application.Interfaces;

public interface IEmployeeService
{
    // TODO: replace object with qualified classes
    Task<object> GetEmployeeLearningPathsAsync(int employeeId);
    Task<IEnumerable<object>> GetAvailableCoursesAsync(int? userId = null);
    Task<IEnumerable<object>> GetAvailableLearningPathsAsync();
    Task<IEnumerable<object>> GetEnrolledCoursesAsync(int userId);
    Task<IEnumerable<object>> GetLearningPathCoursesAsync(int pathId);
    Task<IEnumerable<object>> GetLearningPathCoursesAsync(int userId, int pathId);
    Task<object?> GetCourseDetailsAsync(int userId, int courseId);
    Task<bool> EnrollInCourseAsync(int userId, int courseId);
    Task<bool> EnrollInLearningPathAsync(int userId, int learningPathId);
    Task UpdateCourseProgressAsync(int userId, int courseId, int progress);
    Task<object> RequestCourseCertificateAsync(int userId, int courseId);
    Task<object> RequestLearningPathCertificateAsync(int userId, int learningPathId);
}
