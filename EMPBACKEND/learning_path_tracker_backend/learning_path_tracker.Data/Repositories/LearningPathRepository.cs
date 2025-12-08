using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Data.Repositories;

public class LearningPathRepository : ILearningPathRepository
{
    private readonly AppDbContext _context;

    public LearningPathRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LearningPath>> GetAllAsync()
    {
        return await _context.LearningPaths
            .Include(lp => lp.Modules)
            .Include(lp => lp.LearningPathCourses)
                .ThenInclude(lpc => lpc.Course)
            .ToListAsync();
    }

    public async Task<LearningPath?> GetByIdAsync(int id)
    {
        return await _context.LearningPaths.Include(lp => lp.Modules).FirstOrDefaultAsync(lp => lp.Id == id);
    }

    public async Task<LearningPath> AddAsync(LearningPath learningPath)
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

    public async Task DeleteAsync(int id)
    {
        var learningPath = await _context.LearningPaths.FindAsync(id);
        if (learningPath != null)
        {
            _context.LearningPaths.Remove(learningPath);
            await _context.SaveChangesAsync();
        }
    }
}
