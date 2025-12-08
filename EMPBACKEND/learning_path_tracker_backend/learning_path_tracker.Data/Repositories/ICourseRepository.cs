using learning_path_tracker.Domain.Entities;

namespace learning_path_tracker.Data.Repositories;

public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync();
    Task<Course?> GetByIdAsync(int id);
    Task<Course> AddAsync(Course course);
    Task<Course> UpdateAsync(Course course);
    Task DeleteAsync(int id);
    Task<int> GetActiveCourseCountAsync();
    Task<int> GetCompletedCourseCountAsync();
}
