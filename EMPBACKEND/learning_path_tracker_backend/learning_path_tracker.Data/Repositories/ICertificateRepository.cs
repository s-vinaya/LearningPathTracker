using learning_path_tracker.Domain.Entities;

namespace learning_path_tracker.Data.Repositories;

public interface ICertificateRepository
{
    Task<int> GetTotalCertificatesIssuedAsync();
    Task<Certificate?> GetCourseCertificateAsync(int userId, int courseId);
    Task<Certificate?> GetLearningPathCertificateAsync(int userId, int learningPathId);
    Task<Certificate> CreateCertificateAsync(Certificate certificate);
    Task<List<Certificate>> GetUserCertificatesAsync(int userId);
    Task<Certificate?> GetByCertificateIdAsync(string certificateId);
}
