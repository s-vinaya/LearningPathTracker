using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces.Repositories
{
    public interface ILearningPathRepository
    {
        Task<IEnumerable<LearningPath>> GetAllAsync();
        Task<LearningPath?> GetByIdAsync(int id);
        Task<LearningPath> CreateAsync(LearningPath learningPath);
        Task<LearningPath> UpdateAsync(LearningPath learningPath);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}