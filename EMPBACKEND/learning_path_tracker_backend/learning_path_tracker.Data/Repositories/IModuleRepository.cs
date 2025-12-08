using learning_path_tracker.Domain.Entities;

namespace learning_path_tracker.Data.Repositories;

public interface IModuleRepository
{
    Task<Module?> GetByIdAsync(int id);
    Task<List<Module>> GetByLearningPathIdAsync(int learningPathId);
    Task<Module> AddAsync(Module module);
    Task<Module> UpdateAsync(Module module);
    Task DeleteAsync(int id);
}
