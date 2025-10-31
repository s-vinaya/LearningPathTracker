using EMPBACKEND.Data;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class CourseProgressRepository : ICourseProgressRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseProgressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CourseProgress>> GetAllAsync()
        {
            return await _context.CourseProgresses
                .Include(cp => cp.Enrollment)
                .Include(cp => cp.Course)
                .ToListAsync();
        }

        public async Task<CourseProgress?> GetByIdAsync(int id)
        {
            return await _context.CourseProgresses
                .Include(cp => cp.Enrollment)
                .Include(cp => cp.Course)
                .FirstOrDefaultAsync(cp => cp.ProgressId == id);
        }

        public async Task<CourseProgress> CreateAsync(CourseProgress courseProgress)
        {
            _context.CourseProgresses.Add(courseProgress);
            await _context.SaveChangesAsync();
            return courseProgress;
        }

        public async Task<CourseProgress> UpdateAsync(CourseProgress courseProgress)
        {
            _context.CourseProgresses.Update(courseProgress);
            await _context.SaveChangesAsync();
            return courseProgress;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var courseProgress = await _context.CourseProgresses.FindAsync(id);
            if (courseProgress == null) return false;

            _context.CourseProgresses.Remove(courseProgress);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.CourseProgresses.AnyAsync(cp => cp.ProgressId == id);
        }

        public async Task<IEnumerable<CourseProgress>> GetByEnrollmentIdAsync(int enrollmentId)
        {
            return await _context.CourseProgresses
                .Include(cp => cp.Course)
                .Where(cp => cp.EnrollmentId == enrollmentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseProgress>> GetByCourseIdAsync(int courseId)
        {
            return await _context.CourseProgresses
                .Include(cp => cp.Enrollment)
                .Where(cp => cp.CourseId == courseId)
                .ToListAsync();
        }
    }
}