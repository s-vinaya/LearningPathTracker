using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces
{
    public interface IVideoRequestRepository
    {
        Task<IEnumerable<VideoRequest>> GetAllAsync();
        Task<VideoRequest?> GetByIdAsync(int id);
        Task<IEnumerable<VideoRequest>> GetByUserIdAsync(int userId);
        Task<IEnumerable<VideoRequest>> GetByStatusAsync(string status);
        Task<VideoRequest> CreateAsync(VideoRequest videoRequest);
        Task<VideoRequest> UpdateAsync(VideoRequest videoRequest);
        Task<bool> DeleteAsync(int id);
    }
}