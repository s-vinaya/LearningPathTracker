using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface IUserAssessmentService
    {
        Task<IEnumerable<UserAssessmentDto>> GetAllAsync();
        Task<UserAssessmentDto?> GetByIdAsync(int id);
        Task<IEnumerable<UserAssessmentDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<UserAssessmentDto>> GetByAssessmentIdAsync(int assessmentId);
        Task<UserAssessmentDto> CreateAsync(CreateUserAssessmentDto dto);
        Task<UserAssessmentDto> UpdateAsync(int id, UpdateUserAssessmentDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}