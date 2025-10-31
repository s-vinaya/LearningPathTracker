using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface ILearningPlanService
    {
        Task<IEnumerable<LearningPlanDto>> GetAllAsync();
        Task<LearningPlanDto?> GetByIdAsync(int id);
        Task<IEnumerable<LearningPlanDto>> GetByUserIdAsync(int userId);
        Task<LearningPlanDto> CreateAsync(CreateLearningPlanDto dto);
        Task<LearningPlanDto> UpdateAsync(int id, UpdateLearningPlanDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}