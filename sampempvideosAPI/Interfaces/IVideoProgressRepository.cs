using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces
{
    public interface IVideoProgressRepository
    {
        Task<IEnumerable<VideoProgress>> GetAllAsync();
        Task<VideoProgress?> GetByIdAsync(int id);
        Task<VideoProgress?> GetByUserAndVideoAsync(int userId, int videoId);
        Task<IEnumerable<VideoProgress>> GetByUserIdAsync(int userId);
        Task<VideoProgress> CreateAsync(VideoProgress videoProgress);
        Task<VideoProgress> UpdateAsync(VideoProgress videoProgress);
        Task<bool> DeleteAsync(int id);
    }
}