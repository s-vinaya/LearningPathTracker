using EMPBACKEND.Data;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class LearningPathRepository : ILearningPathRepository
    {
        private readonly ApplicationDbContext _context;

        public LearningPathRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LearningPath>> GetAllAsync()
        {
            return await _context.LearningPaths
                .Include(lp => lp.LearningPathCourses)
                .ThenInclude(lpc => lpc.Course)
                .Where(lp => lp.IsActive)
                .ToListAsync();
        }

        public async Task<LearningPath?> GetByIdAsync(int id)
        {
            return await _context.LearningPaths
                .Include(lp => lp.LearningPathCourses)
                .ThenInclude(lpc => lpc.Course)
                .FirstOrDefaultAsync(lp => lp.Id == id && lp.IsActive);
        }

        public async Task<LearningPath> CreateAsync(LearningPath learningPath)
        {
            _context.LearningPaths.Add(learningPath);
            await _context.SaveChangesAsync();
            return learningPath;
        }

        public async Task<LearningPath> UpdateAsync(LearningPath learningPath)
        {
            _context.LearningPaths.Update(learningPath);
            await _context.SaveChangesAsync();
            return learningPath;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var learningPath = await _context.LearningPaths.FindAsync(id);
            if (learningPath == null) return false;

            learningPath.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.LearningPaths.AnyAsync(lp => lp.Id == id && lp.IsActive);
        }
    }
}