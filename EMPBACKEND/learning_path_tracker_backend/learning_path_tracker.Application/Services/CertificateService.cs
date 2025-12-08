using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Data.Repositories;

namespace learning_path_tracker.Application.Services;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _certificateRepository;

    public CertificateService(ICertificateRepository certificateRepository)
    {
        _certificateRepository = certificateRepository;
    }

    public async Task<CertificateDto?> VerifyCertificateAsync(string certificateId)
    {
        var certificate = await _certificateRepository.GetByCertificateIdAsync(certificateId);
        
        if (certificate == null)
            return null;

        return new CertificateDto
        {
            Id = certificate.Id,
            CertificateId = certificate.CertificateId,
            EmployeeName = certificate.EmployeeName,
            CourseName = certificate.CourseName,
            LearningPathName = certificate.LearningPathName,
            ManagerName = certificate.ManagerName,
            AverageScore = certificate.AverageScore,
            IssuedAt = certificate.IssuedAt,
            CertificateType = certificate.CertificateType
        };
    }
}
