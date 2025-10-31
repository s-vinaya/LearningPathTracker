using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface IUserAssessmentService
    {
        Task<IEnumerable<UserAssessmentDto>> GetAllUserAssessmentsAsync();
        Task<UserAssessmentDto?> GetUserAssessmentByIdAsync(int id);
        Task<IEnumerable<UserAssessmentDto>> GetUserAssessmentsByUserIdAsync(int userId);
        Task<IEnumerable<UserAssessmentDto>> GetUserAssessmentsByAssessmentIdAsync(int assessmentId);
        Task<UserAssessmentDto?> GetUserAssessmentByUserAndAssessmentAsync(int userId, int assessmentId);
        Task<UserAssessmentDto> CreateUserAssessmentAsync(CreateUserAssessmentDto dto);
        Task<UserAssessmentDto> UpdateUserAssessmentAsync(UpdateUserAssessmentDto dto);
        Task<bool> DeleteUserAssessmentAsync(int id);
    }
}