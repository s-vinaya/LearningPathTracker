using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface IOtpCodeService
    {
        Task<IEnumerable<OtpCodeDto>> GetAllAsync();
        Task<OtpCodeDto?> GetByIdAsync(int id);
        Task<OtpCodeDto?> GetByEmailAndCodeAsync(string email, string code);
        Task<OtpCodeDto> CreateAsync(CreateOtpCodeDto dto);
        Task<OtpCodeDto> UpdateAsync(int id, UpdateOtpCodeDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<OtpCodeDto> GenerateOtpAsync(string email);
        Task<bool> VerifyAsync(string email, string code);
        Task<bool> MarkAsUsedAsync(int id);
        Task<bool> DeleteExpiredAsync();
    }
}