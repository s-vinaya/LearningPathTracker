using learning_path_tracker.Domain.Entities;

namespace learning_path_tracker.Data.Repositories;

public interface ILearningPathRepository
{
    Task<List<LearningPath>> GetAllAsync();
    Task<LearningPath?> GetByIdAsync(int id);
    Task<LearningPath> AddAsync(LearningPath learningPath);
    Task<LearningPath> UpdateAsync(LearningPath learningPath);
    Task DeleteAsync(int id);
}
