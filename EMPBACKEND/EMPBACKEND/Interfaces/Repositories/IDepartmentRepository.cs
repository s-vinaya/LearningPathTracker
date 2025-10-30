using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces.Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task<Department> CreateAsync(Department department);
        Task<Department> UpdateAsync(Department department);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}