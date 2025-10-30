using EMPBACKEND.Data;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Models;
using EMPBACKEND.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class DailyGoalRepository : IDailyGoalService
    {
        private readonly ApplicationDbContext _context;

        public DailyGoalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DailyGoalDto>> GetAllAsync()
        {
            var dailyGoals = await _context.DailyGoals.Include(d => d.User).ToListAsync();
            return dailyGoals.Select(MapToDto);
        }

        public async Task<DailyGoalDto?> GetByIdAsync(int id)
        {
            var dailyGoal = await _context.DailyGoals.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == id);
            return dailyGoal != null ? MapToDto(dailyGoal) : null;
        }

        public async Task<IEnumerable<DailyGoalDto>> GetByUserIdAsync(int userId)
        {
            var dailyGoals = await _context.DailyGoals.Where(d => d.UserId == userId).ToListAsync();
            return dailyGoals.Select(MapToDto);
        }

        public async Task<IEnumerable<DailyGoalDto>> GetByUserIdAndDateAsync(int userId, DateTime date)
        {
            var dailyGoals = await _context.DailyGoals
                .Where(d => d.UserId == userId && d.Date.Date == date.Date)
                .ToListAsync();
            return dailyGoals.Select(MapToDto);
        }

        public async Task<DailyGoalDto> CreateAsync(CreateDailyGoalDto createDto)
        {
            var dailyGoal = new DailyGoal
            {
                UserId = createDto.UserId,
                GoalType = createDto.GoalType,
                TargetValue = createDto.TargetValue,
                Date = createDto.Date
            };
            _context.DailyGoals.Add(dailyGoal);
            await _context.SaveChangesAsync();
            return MapToDto(dailyGoal);
        }

        public async Task<DailyGoalDto> UpdateAsync(int id, UpdateDailyGoalDto updateDto)
        {
            var existing = await _context.DailyGoals.FindAsync(id);
            if (existing == null) throw new ArgumentException("DailyGoal not found");
            
            existing.GoalType = updateDto.GoalType;
            existing.TargetValue = updateDto.TargetValue;
            existing.CurrentValue = updateDto.CurrentValue;
            existing.IsCompleted = updateDto.IsCompleted;
            
            await _context.SaveChangesAsync();
            return MapToDto(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dailyGoal = await _context.DailyGoals.FindAsync(id);
            if (dailyGoal == null) return false;

            _context.DailyGoals.Remove(dailyGoal);
            await _context.SaveChangesAsync();
            return true;
        }

        private static DailyGoalDto MapToDto(DailyGoal dailyGoal)
        {
            return new DailyGoalDto
            {
                Id = dailyGoal.Id,
                UserId = dailyGoal.UserId,
                GoalType = dailyGoal.GoalType,
                TargetValue = dailyGoal.TargetValue,
                CurrentValue = dailyGoal.CurrentValue,
                Date = dailyGoal.Date,
                IsCompleted = dailyGoal.IsCompleted
            };
        }
    }
}