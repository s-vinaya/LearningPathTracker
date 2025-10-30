using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllAsync();
        Task<CourseDto?> GetByIdAsync(int id);
        Task<IEnumerable<CourseDto>> GetByCategoryAsync(int categoryId);
        Task<CourseDto> CreateAsync(CreateCourseDto dto);
        Task<CourseDto> UpdateAsync(int id, UpdateCourseDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}