using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces.Repositories
{
    public interface IAssessmentAttemptRepository
    {
        Task<IEnumerable<AssessmentAttempt>> GetAllAsync();
        Task<AssessmentAttempt?> GetByIdAsync(int id);
        Task<AssessmentAttempt> CreateAsync(AssessmentAttempt attempt);
        Task<AssessmentAttempt> UpdateAsync(AssessmentAttempt attempt);
        Task DeleteAsync(int id);
        Task<IEnumerable<AssessmentAttempt>> GetByUserIdAsync(int userId);
        Task<IEnumerable<AssessmentAttempt>> GetByAssessmentIdAsync(int assessmentId);
    }
}