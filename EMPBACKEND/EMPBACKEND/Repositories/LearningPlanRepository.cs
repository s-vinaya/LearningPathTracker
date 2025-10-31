using EMPBACKEND.Data;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class LearningPlanRepository : ILearningPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public LearningPlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LearningPlan>> GetAllAsync()
        {
            return await _context.LearningPlans
                .Include(lp => lp.User)
                .Include(lp => lp.Course)
                .Include(lp => lp.LearningPath)
                .ToListAsync();
        }

        public async Task<LearningPlan?> GetByIdAsync(int id)
        {
            return await _context.LearningPlans
                .Include(lp => lp.User)
                .Include(lp => lp.Course)
                .Include(lp => lp.LearningPath)
                .FirstOrDefaultAsync(lp => lp.Id == id);
        }

        public async Task<IEnumerable<LearningPlan>> GetByUserIdAsync(int userId)
        {
            return await _context.LearningPlans
                .Include(lp => lp.User)
                .Include(lp => lp.Course)
                .Include(lp => lp.LearningPath)
                .Where(lp => lp.UserId == userId)
                .ToListAsync();
        }

        public async Task<LearningPlan> CreateAsync(LearningPlan learningPlan)
        {
            _context.LearningPlans.Add(learningPlan);
            await _context.SaveChangesAsync();
            return learningPlan;
        }

        public async Task<LearningPlan> UpdateAsync(LearningPlan learningPlan)
        {
            _context.LearningPlans.Update(learningPlan);
            await _context.SaveChangesAsync();
            return learningPlan;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var learningPlan = await _context.LearningPlans.FindAsync(id);
            if (learningPlan == null) return false;

            _context.LearningPlans.Remove(learningPlan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.LearningPlans.AnyAsync(lp => lp.Id == id);
        }
    }
}