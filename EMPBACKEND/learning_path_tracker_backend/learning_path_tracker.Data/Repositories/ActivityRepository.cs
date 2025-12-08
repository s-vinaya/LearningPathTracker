using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Data.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly AppDbContext _context;

    public ActivityRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Activity>> GetRecentActivitiesAsync(int count)
    {
        return await _context.Activities
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}
