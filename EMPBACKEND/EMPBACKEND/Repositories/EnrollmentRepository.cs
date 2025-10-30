using EMPBACKEND.Data;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            return await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .Include(e => e.CourseProgresses)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .Include(e => e.CourseProgresses)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Enrollment>> GetByUserIdAsync(int userId)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.CourseProgresses)
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetByCourseIdAsync(int courseId)
        {
            return await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.CourseProgresses)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByUserAndCourseAsync(int userId, int courseId)
        {
            return await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
        }

        public async Task<Enrollment> CreateAsync(Enrollment enrollment)
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

        public async Task<bool> DeleteAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Enrollments.AnyAsync(e => e.Id == id);
        }
    }
}