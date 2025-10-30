using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface IDailyGoalService
    {
        Task<IEnumerable<DailyGoalDto>> GetAllAsync();
        Task<DailyGoalDto?> GetByIdAsync(int id);
        Task<IEnumerable<DailyGoalDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<DailyGoalDto>> GetByUserIdAndDateAsync(int userId, DateTime date);
        Task<DailyGoalDto> CreateAsync(CreateDailyGoalDto dto);
        Task<DailyGoalDto> UpdateAsync(int id, UpdateDailyGoalDto dto);
        Task<bool> DeleteAsync(int id);
    }
}