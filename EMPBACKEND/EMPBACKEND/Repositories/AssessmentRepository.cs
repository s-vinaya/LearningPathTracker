using EMPBACKEND.Data;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AssessmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Assessment>> GetAllAsync()
        {
            return await _context.Assessments.ToListAsync();
        }

        public async Task<Assessment?> GetByIdAsync(int id)
        {
            return await _context.Assessments.FindAsync(id);
        }

        public async Task<Assessment> CreateAsync(Assessment assessment)
        {
            _context.Assessments.Add(assessment);
            await _context.SaveChangesAsync();
            return assessment;
        }

        public async Task<Assessment> UpdateAsync(Assessment assessment)
        {
            _context.Assessments.Update(assessment);
            await _context.SaveChangesAsync();
            return assessment;
        }

        public async Task DeleteAsync(int id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment != null)
            {
                _context.Assessments.Remove(assessment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Assessment>> GetByCourseIdAsync(int courseId)
        {
            return await _context.Assessments.Where(a => a.CourseId == courseId).ToListAsync();
        }
    }
}