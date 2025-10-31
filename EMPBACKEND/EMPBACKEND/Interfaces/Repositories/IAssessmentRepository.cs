using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces.Repositories
{
    public interface IAssessmentRepository
    {
        Task<IEnumerable<Assessment>> GetAllAsync();
        Task<Assessment?> GetByIdAsync(int id);
        Task<Assessment> CreateAsync(Assessment assessment);
        Task<Assessment> UpdateAsync(Assessment assessment);
        Task DeleteAsync(int id);
        Task<IEnumerable<Assessment>> GetByCourseIdAsync(int courseId);
    }
}