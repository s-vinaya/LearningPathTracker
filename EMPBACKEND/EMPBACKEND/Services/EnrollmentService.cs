using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;

namespace EMPBACKEND.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repository;

        public EnrollmentService(IEnrollmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EnrollmentDto>> GetAllAsync()
        {
            var enrollments = await _repository.GetAllAsync();
            return enrollments.Select(MapToDto);
        }

        public async Task<EnrollmentDto?> GetByIdAsync(int id)
        {
            var enrollment = await _repository.GetByIdAsync(id);
            return enrollment != null ? MapToDto(enrollment) : null;
        }

        public async Task<IEnumerable<EnrollmentDto>> GetByUserIdAsync(int userId)
        {
            var enrollments = await _repository.GetByUserIdAsync(userId);
            return enrollments.Select(MapToDto);
        }

        public async Task<IEnumerable<EnrollmentDto>> GetByCourseIdAsync(int courseId)
        {
            var enrollments = await _repository.GetByCourseIdAsync(courseId);
            return enrollments.Select(MapToDto);
        }

        public async Task<EnrollmentDto> CreateAsync(CreateEnrollmentDto createDto)
        {
            var enrollment = new Enrollment
            {
                UserId = createDto.UserId,
                CourseId = createDto.CourseId,
                EnrolledDate = DateTime.UtcNow,
                Status = "Enrolled",
                Progress = 0
            };

            var created = await _repository.CreateAsync(enrollment);
            var result = await _repository.GetByIdAsync(created.Id);
            return MapToDto(result!);
        }

        public async Task<EnrollmentDto> UpdateAsync(int id, UpdateEnrollmentDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) throw new ArgumentException("Enrollment not found");

            existing.Status = updateDto.Status;
            existing.Progress = updateDto.Progress;
            existing.CompletionDate = updateDto.CompletionDate;

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

        private static EnrollmentDto MapToDto(Enrollment enrollment)
        {
            return new EnrollmentDto
            {
                Id = enrollment.Id,
                UserId = enrollment.UserId,
                UserName = enrollment.User?.Username ?? string.Empty,
                CourseId = enrollment.CourseId,
                CourseName = enrollment.Course?.Title ?? string.Empty,
                EnrolledDate = enrollment.EnrolledDate,
                CompletionDate = enrollment.CompletionDate,
                Status = enrollment.Status,
                Progress = enrollment.Progress
            };
        }
    }
}