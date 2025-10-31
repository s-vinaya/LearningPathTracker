using EMPBACKEND.Data;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class AssessmentAttemptRepository : IAssessmentAttemptRepository
    {
        private readonly ApplicationDbContext _context;

        public AssessmentAttemptRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AssessmentAttempt>> GetAllAsync()
        {
            return await _context.AssessmentAttempts.Include(a => a.Assessment).Include(u => u.User).ToListAsync();
        }

        public async Task<AssessmentAttempt?> GetByIdAsync(int id)
        {
            return await _context.AssessmentAttempts.Include(a => a.Assessment).Include(u => u.User).FirstOrDefaultAsync(a => a.AttemptId == id);
        }

        public async Task<AssessmentAttempt> CreateAsync(AssessmentAttempt attempt)
        {
            _context.AssessmentAttempts.Add(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }

        public async Task<AssessmentAttempt> UpdateAsync(AssessmentAttempt attempt)
        {
            _context.AssessmentAttempts.Update(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }

        public async Task DeleteAsync(int id)
        {
            var attempt = await _context.AssessmentAttempts.FindAsync(id);
            if (attempt != null)
            {
                _context.AssessmentAttempts.Remove(attempt);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AssessmentAttempt>> GetByUserIdAsync(int userId)
        {
            return await _context.AssessmentAttempts.Include(a => a.Assessment).Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<AssessmentAttempt>> GetByAssessmentIdAsync(int assessmentId)
        {
            return await _context.AssessmentAttempts.Include(a => a.Assessment).Where(a => a.AssessmentId == assessmentId).ToListAsync();
        }
    }
}