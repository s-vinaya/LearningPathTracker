using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces.Repositories
{
    public interface ILearningPlanRepository
    {
        Task<IEnumerable<LearningPlan>> GetAllAsync();
        Task<LearningPlan?> GetByIdAsync(int id);
        Task<IEnumerable<LearningPlan>> GetByUserIdAsync(int userId);
        Task<LearningPlan> CreateAsync(LearningPlan learningPlan);
        Task<LearningPlan> UpdateAsync(LearningPlan learningPlan);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}