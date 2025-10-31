using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface IAssessmentAttemptService
    {
        Task<IEnumerable<AssessmentAttemptDto>> GetAllAsync();
        Task<AssessmentAttemptDto?> GetByIdAsync(int id);
        Task<AssessmentAttemptDto> CreateAsync(CreateAssessmentAttemptDto attemptDto);
        Task<AssessmentAttemptDto> UpdateAsync(int id, AssessmentAttemptDto attemptDto);
        Task DeleteAsync(int id);
        Task<IEnumerable<AssessmentAttemptDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<AssessmentAttemptDto>> GetByAssessmentIdAsync(int assessmentId);
    }
}