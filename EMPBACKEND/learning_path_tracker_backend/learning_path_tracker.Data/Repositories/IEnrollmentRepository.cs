using learning_path_tracker.Domain.Entities;

namespace learning_path_tracker.Data.Repositories;

public interface IEnrollmentRepository
{
    Task<List<Enrollment>> GetAllAsync();
    Task<List<Enrollment>> GetByUserIdAsync(int userId);
    Task<List<Enrollment>> GetByCourseIdAsync(int courseId);
    Task<List<Enrollment>> GetCompletionsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<Enrollment>> GetEnrollmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<int> GetTotalEnrollmentsAsync();
    Task<int> GetTotalCompletionsAsync();
    Task<Enrollment> AddAsync(Enrollment enrollment);
    Task<Enrollment> UpdateAsync(Enrollment enrollment);
}
