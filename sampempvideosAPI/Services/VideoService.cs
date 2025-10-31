using EMPBACKEND.Interfaces;
using EMPBACKEND.Models;

namespace EMPBACKEND.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepository;

        public VideoService(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async Task<IEnumerable<Video>> GetAllVideosAsync()
        {
            return await _videoRepository.GetAllAsync();
        }

        public async Task<Video?> GetVideoByIdAsync(int id)
        {
            return await _videoRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Video>> GetVideosByCourseAsync(int courseId)
        {
            return await _videoRepository.GetByCourseIdAsync(courseId);
        }

        public async Task<Video> CreateVideoAsync(Video video)
        {
            video.CreatedDate = DateTime.UtcNow;
            return await _videoRepository.CreateAsync(video);
        }

        public async Task<Video> UpdateVideoAsync(Video video)
        {
            var existingVideo = await _videoRepository.GetByIdAsync(video.Id);
            if (existingVideo == null)
                throw new ArgumentException("Video not found");

            return await _videoRepository.UpdateAsync(video);
        }

        public async Task<bool> DeleteVideoAsync(int id)
        {
            return await _videoRepository.DeleteAsync(id);
        }
    }
}