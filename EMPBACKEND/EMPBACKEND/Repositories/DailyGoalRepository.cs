using EMPBACKEND.Data;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class DailyGoalRepository : IDailyGoalRepository
    {
        private readonly ApplicationDbContext _context;

        public DailyGoalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DailyGoal>> GetAllAsync()
        {
            return await _context.DailyGoals.Include(d => d.User).ToListAsync();
        }

        public async Task<DailyGoal?> GetByIdAsync(int id)
        {
            return await _context.DailyGoals.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<DailyGoal>> GetByUserIdAsync(int userId)
        {
            return await _context.DailyGoals.Where(d => d.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<DailyGoal>> GetByDateAsync(DateTime date)
        {
            return await _context.DailyGoals.Where(d => d.Date.Date == date.Date).ToListAsync();
        }

        public async Task<IEnumerable<DailyGoal>> GetByUserAndDateAsync(int userId, DateTime date)
        {
            return await _context.DailyGoals
                .Where(d => d.UserId == userId && d.Date.Date == date.Date)
                .ToListAsync();
        }

        public async Task<DailyGoal> CreateAsync(DailyGoal dailyGoal)
        {
            _context.DailyGoals.Add(dailyGoal);
            await _context.SaveChangesAsync();
            return dailyGoal;
        }

        public async Task<DailyGoal> UpdateAsync(DailyGoal dailyGoal)
        {
            _context.DailyGoals.Update(dailyGoal);
            await _context.SaveChangesAsync();
            return dailyGoal;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dailyGoal = await _context.DailyGoals.FindAsync(id);
            if (dailyGoal == null) return false;

            _context.DailyGoals.Remove(dailyGoal);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.DailyGoals.AnyAsync(d => d.Id == id);
        }
    }
}