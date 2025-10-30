using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface ICertificateService
    {
        Task<IEnumerable<CertificateDto>> GetAllAsync();
        Task<CertificateDto?> GetByIdAsync(int id);
        Task<IEnumerable<CertificateDto>> GetByUserIdAsync(int userId);
        Task<CertificateDto> CreateAsync(CreateCertificateDto dto);
        Task<CertificateDto> UpdateAsync(int id, UpdateCertificateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}