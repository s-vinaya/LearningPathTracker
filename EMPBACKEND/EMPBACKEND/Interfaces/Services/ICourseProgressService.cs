using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface ICourseProgressService
    {
        Task<IEnumerable<CourseProgressDto>> GetAllAsync();
        Task<CourseProgressDto?> GetByIdAsync(int id);
        Task<IEnumerable<CourseProgressDto>> GetByEnrollmentIdAsync(int enrollmentId);
        Task<IEnumerable<CourseProgressDto>> GetByCourseIdAsync(int courseId);
        Task<CourseProgressDto> UpdateAsync(int id, UpdateCourseProgressDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}