using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces.Repositories
{
    public interface IDailyGoalRepository
    {
        Task<IEnumerable<DailyGoal>> GetAllAsync();
        Task<DailyGoal?> GetByIdAsync(int id);
        Task<DailyGoal> CreateAsync(DailyGoal dailyGoal);
        Task<DailyGoal> UpdateAsync(DailyGoal dailyGoal);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<DailyGoal>> GetByUserIdAsync(int userId);
        Task<IEnumerable<DailyGoal>> GetByDateAsync(DateTime date);
        Task<IEnumerable<DailyGoal>> GetByUserAndDateAsync(int userId, DateTime date);
    }
}