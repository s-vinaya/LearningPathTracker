using EMPBACKEND.Interfaces;
using EMPBACKEND.Models;

namespace EMPBACKEND.Services
{
    public class VideoProgressService : IVideoProgressService
    {
        private readonly IVideoProgressRepository _progressRepository;
        private readonly IVideoRepository _videoRepository;

        public VideoProgressService(IVideoProgressRepository progressRepository, IVideoRepository videoRepository)
        {
            _progressRepository = progressRepository;
            _videoRepository = videoRepository;
        }

        public async Task<VideoProgress?> GetProgressAsync(int userId, int videoId)
        {
            return await _progressRepository.GetByUserAndVideoAsync(userId, videoId);
        }

        public async Task<IEnumerable<VideoProgress>> GetUserProgressAsync(int userId)
        {
            return await _progressRepository.GetByUserIdAsync(userId);
        }

        public async Task<VideoProgress> UpdateProgressAsync(int userId, int videoId, int watchedDuration)
        {
            var progress = await _progressRepository.GetByUserAndVideoAsync(userId, videoId);
            var video = await _videoRepository.GetByIdAsync(videoId);

            if (video == null)
                throw new ArgumentException("Video not found");

            if (progress == null)
            {
                progress = new VideoProgress
                {
                    UserId = userId,
                    VideoId = videoId,
                    WatchedDuration = watchedDuration,
                    LastWatchedDate = DateTime.UtcNow,
                    IsCompleted = watchedDuration >= video.Duration
                };
                return await _progressRepository.CreateAsync(progress);
            }

            progress.WatchedDuration = Math.Max(progress.WatchedDuration, watchedDuration);
            progress.LastWatchedDate = DateTime.UtcNow;
            progress.IsCompleted = progress.WatchedDuration >= video.Duration;

            return await _progressRepository.UpdateAsync(progress);
        }

        public async Task<VideoProgress> MarkVideoCompleteAsync(int userId, int videoId)
        {
            var video = await _videoRepository.GetByIdAsync(videoId);
            if (video == null)
                throw new ArgumentException("Video not found");

            return await UpdateProgressAsync(userId, videoId, video.Duration);
        }

        public async Task<decimal> GetCourseProgressAsync(int userId, int courseId)
        {
            var courseVideos = await _videoRepository.GetByCourseIdAsync(courseId);
            var userProgress = await _progressRepository.GetByUserIdAsync(userId);

            var videoIds = courseVideos.Select(v => v.Id).ToList();
            var completedVideos = userProgress.Count(p => videoIds.Contains(p.VideoId) && p.IsCompleted);

            return videoIds.Count == 0 ? 0 : (decimal)completedVideos / videoIds.Count * 100;
        }
    }
}