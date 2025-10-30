using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;

namespace EMPBACKEND.Services
{
    public class CourseProgressService : ICourseProgressService
    {
        private readonly ICourseProgressRepository _repository;

        public CourseProgressService(ICourseProgressRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CourseProgressDto>> GetAllAsync()
        {
            var courseProgresses = await _repository.GetAllAsync();
            return courseProgresses.Select(MapToDto);
        }

        public async Task<CourseProgressDto?> GetByIdAsync(int id)
        {
            var courseProgress = await _repository.GetByIdAsync(id);
            return courseProgress != null ? MapToDto(courseProgress) : null;
        }

        public async Task<IEnumerable<CourseProgressDto>> GetByEnrollmentIdAsync(int enrollmentId)
        {
            var courseProgresses = await _repository.GetByEnrollmentIdAsync(enrollmentId);
            return courseProgresses.Select(MapToDto);
        }

        public async Task<IEnumerable<CourseProgressDto>> GetByCourseIdAsync(int courseId)
        {
            var courseProgresses = await _repository.GetByCourseIdAsync(courseId);
            return courseProgresses.Select(MapToDto);
        }

        public async Task<CourseProgressDto> UpdateAsync(int id, UpdateCourseProgressDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) throw new ArgumentException("CourseProgress not found");

            existing.Status = updateDto.Status;
            existing.PercentComplete = updateDto.PercentComplete;
            existing.TimeSpentMinutes = updateDto.TimeSpentMinutes;
            existing.LastAccessed = DateTime.UtcNow;

            if (updateDto.Status == "Completed" && existing.CompletedAt == null)
            {
                existing.CompletedAt = DateTime.UtcNow;
            }

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

        private static CourseProgressDto MapToDto(CourseProgress courseProgress)
        {
            return new CourseProgressDto
            {
                ProgressId = courseProgress.ProgressId,
                EnrollmentId = courseProgress.EnrollmentId,
                CourseId = courseProgress.CourseId,
                Status = courseProgress.Status,
                PercentComplete = courseProgress.PercentComplete,
                LastAccessed = courseProgress.LastAccessed,
                CompletedAt = courseProgress.CompletedAt,
                TimeSpentMinutes = courseProgress.TimeSpentMinutes
            };
        }
    }
}