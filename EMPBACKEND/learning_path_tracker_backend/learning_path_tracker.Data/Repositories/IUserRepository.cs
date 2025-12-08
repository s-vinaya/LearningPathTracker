using learning_path_tracker.Domain.Entities;

namespace learning_path_tracker.Data.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByIdWithEnrollmentsAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetUsersByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<User> AddAsync(User user);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task<int> GetTotalUserCountAsync();
}
