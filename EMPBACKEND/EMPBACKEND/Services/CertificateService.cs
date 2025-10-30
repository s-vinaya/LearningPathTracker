using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Models;

namespace EMPBACKEND.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _repository;

        public CertificateService(ICertificateRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CertificateDto>> GetAllAsync()
        {
            var certificates = await _repository.GetAllAsync();
            return certificates.Select(c => new CertificateDto
            {
                CertificateId = c.CertificateId,
                UserId = c.UserId,
                LearningPathId = c.LearningPathId,
                CertificateNumber = c.CertificateNumber,
                IssuedAt = c.IssuedAt,
                ExpiresAt = c.ExpiresAt,
                CertificateUrl = c.CertificateUrl,
                IsActive = c.IsActive
            });
        }

        public async Task<CertificateDto?> GetByIdAsync(int id)
        {
            var certificate = await _repository.GetByIdAsync(id);
            return certificate != null ? new CertificateDto
            {
                CertificateId = certificate.CertificateId,
                UserId = certificate.UserId,
                LearningPathId = certificate.LearningPathId,
                CertificateNumber = certificate.CertificateNumber,
                IssuedAt = certificate.IssuedAt,
                ExpiresAt = certificate.ExpiresAt,
                CertificateUrl = certificate.CertificateUrl,
                IsActive = certificate.IsActive
            } : null;
        }

        public async Task<CertificateDto> CreateAsync(CreateCertificateDto certificateDto)
        {
            var certificate = new Certificate
            {
                UserId = certificateDto.UserId,
                LearningPathId = certificateDto.LearningPathId,
                ExpiresAt = certificateDto.ExpiresAt,
                IssuedAt = DateTime.UtcNow,
                CertificateNumber = Guid.NewGuid().ToString(),
                IsActive = true
            };
            var created = await _repository.CreateAsync(certificate);
            return new CertificateDto
            {
                CertificateId = created.CertificateId,
                UserId = created.UserId,
                LearningPathId = created.LearningPathId,
                CertificateNumber = created.CertificateNumber,
                IssuedAt = created.IssuedAt,
                ExpiresAt = created.ExpiresAt,
                CertificateUrl = created.CertificateUrl,
                IsActive = created.IsActive
            };
        }

        public async Task<CertificateDto> UpdateAsync(int id, UpdateCertificateDto certificateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) throw new ArgumentException("Certificate not found");
            
            existing.ExpiresAt = certificateDto.ExpiresAt;
            existing.IsActive = certificateDto.IsActive;
            
            var updated = await _repository.UpdateAsync(existing);
            return new CertificateDto
            {
                CertificateId = updated.CertificateId,
                UserId = updated.UserId,
                LearningPathId = updated.LearningPathId,
                CertificateNumber = updated.CertificateNumber,
                IssuedAt = updated.IssuedAt,
                ExpiresAt = updated.ExpiresAt,
                CertificateUrl = updated.CertificateUrl,
                IsActive = updated.IsActive
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<IEnumerable<CertificateDto>> GetByUserIdAsync(int userId)
        {
            var certificates = await _repository.GetByUserIdAsync(userId);
            return certificates.Select(c => new CertificateDto
            {
                CertificateId = c.CertificateId,
                UserId = c.UserId,
                LearningPathId = c.LearningPathId,
                CertificateNumber = c.CertificateNumber,
                IssuedAt = c.IssuedAt,
                ExpiresAt = c.ExpiresAt,
                CertificateUrl = c.CertificateUrl,
                IsActive = c.IsActive
            });
        }
    }
}