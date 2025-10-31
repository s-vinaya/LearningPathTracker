using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces.Repositories
{
    public interface IVideoProgressRepository
    {
        Task<IEnumerable<VideoProgress>> GetAllAsync();
        Task<VideoProgress?> GetByIdAsync(int id);
        Task<IEnumerable<VideoProgress>> GetByUserIdAsync(int userId);
        Task<IEnumerable<VideoProgress>> GetByVideoIdAsync(int videoId);
        Task<VideoProgress?> GetByUserAndVideoAsync(int userId, int videoId);
        Task<VideoProgress> CreateAsync(VideoProgress videoProgress);
        Task<VideoProgress> UpdateAsync(VideoProgress videoProgress);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}