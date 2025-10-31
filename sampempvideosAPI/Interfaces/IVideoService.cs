using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces
{
    public interface IVideoService
    {
        Task<IEnumerable<Video>> GetAllVideosAsync();
        Task<Video?> GetVideoByIdAsync(int id);
        Task<IEnumerable<Video>> GetVideosByCourseAsync(int courseId);
        Task<Video> CreateVideoAsync(Video video);
        Task<Video> UpdateVideoAsync(Video video);
        Task<bool> DeleteVideoAsync(int id);
    }
}