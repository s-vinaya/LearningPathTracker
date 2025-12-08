using learning_path_tracker.Domain.Entities;

namespace learning_path_tracker.Data.Repositories;

public interface IActivityRepository
{
    Task<List<Activity>> GetRecentActivitiesAsync(int count);
}
