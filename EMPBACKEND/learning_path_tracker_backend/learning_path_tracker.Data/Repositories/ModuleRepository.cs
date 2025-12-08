using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Data.Repositories;

public class ModuleRepository : IModuleRepository
{
    private readonly AppDbContext _context;

    public ModuleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Module?> GetByIdAsync(int id)
    {
        return await _context.Modules.FindAsync(id);
    }

    public async Task<List<Module>> GetByLearningPathIdAsync(int learningPathId)
    {
        return await _context.Modules
            .Where(m => m.LearningPathId == learningPathId)
            .ToListAsync();
    }

    public async Task<Module> AddAsync(Module module)
    {
        _context.Modules.Add(module);
        await _context.SaveChangesAsync();
        return module;
    }

    public async Task<Module> UpdateAsync(Module module)
    {
        _context.Modules.Update(module);
        await _context.SaveChangesAsync();
        return module;
    }

    public async Task DeleteAsync(int id)
    {
        var module = await _context.Modules.FindAsync(id);
        if (module != null)
        {
            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
        }
    }
}
