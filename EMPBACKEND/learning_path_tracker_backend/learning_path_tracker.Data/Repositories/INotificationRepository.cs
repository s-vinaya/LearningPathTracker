using learning_path_tracker.Domain.Entities;

namespace learning_path_tracker.Data.Repositories;

public interface INotificationRepository
{
    Task<List<Notification>> GetAllAsync();
    Task<Notification> AddAsync(Notification notification);
    Task<bool> DeleteAsync(int id);
    Task<Notification?> GetByIdAsync(int id);
}
