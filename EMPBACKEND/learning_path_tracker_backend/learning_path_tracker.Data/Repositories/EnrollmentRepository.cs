using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Data.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly AppDbContext _context;

    public EnrollmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Enrollment>> GetAllAsync()
    {
        return await _context.Enrollments.Include(e => e.Course).ToListAsync();
    }

    public async Task<List<Enrollment>> GetByUserIdAsync(int userId)
    {
        return await _context.Enrollments.Where(e => e.UserId == userId).ToListAsync();
    }

    public async Task<List<Enrollment>> GetByCourseIdAsync(int courseId)
    {
        return await _context.Enrollments.Where(e => e.CourseId == courseId).ToListAsync();
    }

    public async Task<List<Enrollment>> GetCompletionsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.Progress >= 80 && 
                       (!e.Course.QuizId.HasValue || e.QuizPassed) &&
                       e.EnrolledAt >= startDate && e.EnrolledAt <= endDate)
            .ToListAsync();
    }

    public async Task<List<Enrollment>> GetEnrollmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Enrollments
            .Where(e => e.EnrolledAt >= startDate && e.EnrolledAt <= endDate)
            .ToListAsync();
    }

    public async Task<int> GetTotalEnrollmentsAsync()
    {
        return await _context.Enrollments.CountAsync();
    }

    public async Task<int> GetTotalCompletionsAsync()
    {
        return await _context.Enrollments.CountAsync(e => e.CompletedAt != null);
    }

    public async Task<Enrollment> AddAsync(Enrollment enrollment)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }

    public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
    {
        _context.Enrollments.Update(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }
}
