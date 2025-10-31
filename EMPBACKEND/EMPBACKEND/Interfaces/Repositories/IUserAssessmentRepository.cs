using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces.Repositories
{
    public interface IUserAssessmentRepository
    {
        Task<IEnumerable<UserAssessment>> GetAllAsync();
        Task<UserAssessment?> GetByIdAsync(int id);
        Task<IEnumerable<UserAssessment>> GetByUserIdAsync(int userId);
        Task<IEnumerable<UserAssessment>> GetByAssessmentIdAsync(int assessmentId);
        Task<UserAssessment?> GetByUserAndAssessmentAsync(int userId, int assessmentId);
        Task<UserAssessment> CreateAsync(UserAssessment userAssessment);
        Task<UserAssessment> UpdateAsync(UserAssessment userAssessment);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}