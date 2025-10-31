using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces.Repositories
{
    public interface IOtpCodeRepository
    {
        Task<IEnumerable<OtpCode>> GetAllAsync();
        Task<OtpCode?> GetByIdAsync(int id);
        Task<OtpCode?> GetByEmailAndCodeAsync(string email, string code);
        Task<OtpCode> CreateAsync(OtpCode otpCode);
        Task<OtpCode> UpdateAsync(OtpCode otpCode);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}