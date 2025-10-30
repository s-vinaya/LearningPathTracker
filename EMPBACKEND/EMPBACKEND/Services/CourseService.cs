using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;

namespace EMPBACKEND.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;

        public CourseService(ICourseRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CourseDto>> GetAllAsync()
        {
            var courses = await _repository.GetAllAsync();
            return courses.Select(MapToDto);
        }

        public async Task<CourseDto?> GetByIdAsync(int id)
        {
            var course = await _repository.GetByIdAsync(id);
            return course != null ? MapToDto(course) : null;
        }

        public async Task<IEnumerable<CourseDto>> GetByCategoryAsync(int categoryId)
        {
            var courses = await _repository.GetByCategoryAsync(categoryId);
            return courses.Select(MapToDto);
        }

        public async Task<CourseDto> CreateAsync(CreateCourseDto createDto)
        {
            var course = new Course
            {
                Title = createDto.Title,
                Description = createDto.Description,
                CategoryId = createDto.CategoryId,
                Duration = createDto.Duration,
                VideoUrl = createDto.VideoUrl,
                CreatedBy = createDto.CreatedBy,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(course);
            var result = await _repository.GetByIdAsync(created.Id);
            return MapToDto(result!);
        }

        public async Task<CourseDto> UpdateAsync(int id, UpdateCourseDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) throw new ArgumentException("Course not found");

            existing.Title = updateDto.Title;
            existing.Description = updateDto.Description;
            existing.CategoryId = updateDto.CategoryId;
            existing.Duration = updateDto.Duration;
            existing.VideoUrl = updateDto.VideoUrl;
            existing.IsActive = updateDto.IsActive;

            await _repository.UpdateAsync(existing);
            var updated = await _repository.GetByIdAsync(id);
            return MapToDto(updated!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        private static CourseDto MapToDto(Course course)
        {
            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                CategoryId = course.CategoryId,
                CategoryName = course.Category?.Name ?? string.Empty,
                Duration = course.Duration,
                CreatedBy = course.CreatedBy,
                IsActive = course.IsActive,
                CreatedDate = course.CreatedDate
            };
        }
    }
}