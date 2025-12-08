using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Data.Repositories;

public class CertificateRepository : ICertificateRepository
{
    private readonly AppDbContext _context;

    public CertificateRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetTotalCertificatesIssuedAsync()
    {
        return await _context.Certificates.CountAsync();
    }

    public async Task<Certificate?> GetCourseCertificateAsync(int userId, int courseId)
    {
        return await _context.Certificates
            .FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId && c.CertificateType == "Course");
    }

    public async Task<Certificate?> GetLearningPathCertificateAsync(int userId, int learningPathId)
    {
        return await _context.Certificates
            .FirstOrDefaultAsync(c => c.UserId == userId && c.LearningPathId == learningPathId && c.CertificateType == "LearningPath");
    }

    public async Task<Certificate> CreateCertificateAsync(Certificate certificate)
    {
        _context.Certificates.Add(certificate);
        await _context.SaveChangesAsync();
        return certificate;
    }

    public async Task<List<Certificate>> GetUserCertificatesAsync(int userId)
    {
        return await _context.Certificates
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.IssuedAt)
            .ToListAsync();
    }

    public async Task<Certificate?> GetByCertificateIdAsync(string certificateId)
    {
        return await _context.Certificates
            .Include(c => c.User)
            .Include(c => c.Course)
            .Include(c => c.LearningPath)
            .FirstOrDefaultAsync(c => c.CertificateId == certificateId);
    }
}
