using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces
{
    public interface IVideoProgressService
    {
        Task<VideoProgress?> GetProgressAsync(int userId, int videoId);
        Task<IEnumerable<VideoProgress>> GetUserProgressAsync(int userId);
        Task<VideoProgress> UpdateProgressAsync(int userId, int videoId, int watchedDuration);
        Task<VideoProgress> MarkVideoCompleteAsync(int userId, int videoId);
        Task<decimal> GetCourseProgressAsync(int userId, int courseId);
    }
}