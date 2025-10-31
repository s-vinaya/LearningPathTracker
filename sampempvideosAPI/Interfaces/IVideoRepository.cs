using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces
{
    public interface IVideoRepository
    {
        Task<IEnumerable<Video>> GetAllAsync();
        Task<Video?> GetByIdAsync(int id);
        Task<IEnumerable<Video>> GetByCourseIdAsync(int courseId);
        Task<Video> CreateAsync(Video video);
        Task<Video> UpdateAsync(Video video);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}