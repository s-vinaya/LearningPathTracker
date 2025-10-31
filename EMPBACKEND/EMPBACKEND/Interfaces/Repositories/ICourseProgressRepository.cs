using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces.Repositories
{
    public interface ICourseProgressRepository
    {
        Task<IEnumerable<CourseProgress>> GetAllAsync();
        Task<CourseProgress?> GetByIdAsync(int id);
        Task<CourseProgress> CreateAsync(CourseProgress courseProgress);
        Task<CourseProgress> UpdateAsync(CourseProgress courseProgress);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<CourseProgress>> GetByEnrollmentIdAsync(int enrollmentId);
        Task<IEnumerable<CourseProgress>> GetByCourseIdAsync(int courseId);
    }
}