using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface ILearningPathService
    {
        Task<IEnumerable<LearningPathDto>> GetAllAsync();
        Task<LearningPathDto?> GetByIdAsync(int id);
        Task<LearningPathDto> CreateAsync(CreateLearningPathDto dto);
        Task<LearningPathDto> UpdateAsync(int id, UpdateLearningPathDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<LearningPathCourseDto> AddCourseToPathAsync(int pathId, CreateLearningPathCourseDto courseDto);
        Task<bool> RemoveCourseFromPathAsync(int pathId, int courseId);
        Task<IEnumerable<LearningPathCourseDto>> GetLearningPathCoursesAsync(int pathId);
    }
}