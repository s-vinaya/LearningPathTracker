using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<EnrollmentDto>> GetAllAsync();
        Task<EnrollmentDto?> GetByIdAsync(int id);
        Task<IEnumerable<EnrollmentDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<EnrollmentDto>> GetByCourseIdAsync(int courseId);
        Task<EnrollmentDto> CreateAsync(CreateEnrollmentDto dto);
        Task<EnrollmentDto> UpdateAsync(int id, UpdateEnrollmentDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}