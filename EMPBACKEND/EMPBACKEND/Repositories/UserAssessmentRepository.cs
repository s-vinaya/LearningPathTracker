using EMPBACKEND.Data;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class UserAssessmentRepository : IUserAssessmentRepository
    {
        private readonly ApplicationDbContext _context;

        public UserAssessmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserAssessment>> GetAllAsync()
        {
            return await _context.UserAssessments
                .Include(ua => ua.User)
                .Include(ua => ua.Assessment)
                .ToListAsync();
        }

        public async Task<UserAssessment?> GetByIdAsync(int id)
        {
            return await _context.UserAssessments
                .Include(ua => ua.User)
                .Include(ua => ua.Assessment)
                .FirstOrDefaultAsync(ua => ua.Id == id);
        }

        public async Task<IEnumerable<UserAssessment>> GetByUserIdAsync(int userId)
        {
            return await _context.UserAssessments
                .Include(ua => ua.User)
                .Include(ua => ua.Assessment)
                .Where(ua => ua.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserAssessment>> GetByAssessmentIdAsync(int assessmentId)
        {
            return await _context.UserAssessments
                .Include(ua => ua.User)
                .Include(ua => ua.Assessment)
                .Where(ua => ua.AssessmentId == assessmentId)
                .ToListAsync();
        }

        public async Task<UserAssessment?> GetByUserAndAssessmentAsync(int userId, int assessmentId)
        {
            return await _context.UserAssessments
                .Include(ua => ua.User)
                .Include(ua => ua.Assessment)
                .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AssessmentId == assessmentId);
        }

        public async Task<UserAssessment> CreateAsync(UserAssessment userAssessment)
        {
            _context.UserAssessments.Add(userAssessment);
            await _context.SaveChangesAsync();
            return userAssessment;
        }

        public async Task<UserAssessment> UpdateAsync(UserAssessment userAssessment)
        {
            var existingUserAssessment = await _context.UserAssessments.FindAsync(userAssessment.Id);
            if (existingUserAssessment == null) throw new ArgumentException("UserAssessment not found");

            existingUserAssessment.UserId = userAssessment.UserId;
            existingUserAssessment.AssessmentId = userAssessment.AssessmentId;
            existingUserAssessment.Score = userAssessment.Score;
            existingUserAssessment.IsPassed = userAssessment.IsPassed;
            existingUserAssessment.AttemptDate = userAssessment.AttemptDate;

            await _context.SaveChangesAsync();
            return existingUserAssessment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var userAssessment = await _context.UserAssessments.FindAsync(id);
            if (userAssessment == null) return false;

            _context.UserAssessments.Remove(userAssessment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.UserAssessments.AnyAsync(ua => ua.Id == id);
        }
    }
}