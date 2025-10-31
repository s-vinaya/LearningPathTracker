using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces.Repositories
{
    public interface ICertificateRepository
    {
        Task<IEnumerable<Certificate>> GetAllAsync();
        Task<Certificate?> GetByIdAsync(int id);
        Task<IEnumerable<Certificate>> GetByUserIdAsync(int userId);
        Task<Certificate> CreateAsync(Certificate certificate);
        Task<Certificate> UpdateAsync(Certificate certificate);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}