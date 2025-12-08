using learning_path_tracker.Application.DTOs;

namespace learning_path_tracker.Application.Interfaces;

public interface ICertificateService
{
    Task<CertificateDto?> VerifyCertificateAsync(string certificateId);
}
