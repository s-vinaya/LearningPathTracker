using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
<<<<<<< Updated upstream
=======
using AutoMapper;
>>>>>>> Stashed changes

namespace EMPBACKEND.Services
{
    public class VideoProgressService : IVideoProgressService
    {
        private readonly IVideoProgressRepository _repository;
<<<<<<< Updated upstream

        public VideoProgressService(IVideoProgressRepository repository)
        {
            _repository = repository;
=======
        private readonly IMapper _mapper;

        public VideoProgressService(IVideoProgressRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
>>>>>>> Stashed changes
        }

        public async Task<IEnumerable<VideoProgressDto>> GetAllVideoProgressAsync()
        {
            var videosProgress = await _repository.GetAllAsync();
            return videosProgress.Select(vp => new VideoProgressDto
            {
                Id = vp.Id,
                UserId = vp.UserId,
                UserName = vp.User?.FirstName + " " + vp.User?.LastName ?? "Unknown",
                VideoId = vp.VideoId,
                VideoTitle = vp.Video?.Title ?? "Unknown",
                WatchedDuration = vp.WatchedDuration,
                LastWatchedDate = vp.LastWatchedDate,
                IsCompleted = vp.IsCompleted
            });
        }

        public async Task<VideoProgressDto?> GetVideoProgressByIdAsync(int id)
        {
            var videoProgress = await _repository.GetByIdAsync(id);
            if (videoProgress == null) return null;

            return new VideoProgressDto
            {
                Id = videoProgress.Id,
                UserId = videoProgress.UserId,
                UserName = videoProgress.User?.FirstName + " " + videoProgress.User?.LastName ?? "Unknown",
                VideoId = videoProgress.VideoId,
                VideoTitle = videoProgress.Video?.Title ?? "Unknown",
                WatchedDuration = videoProgress.WatchedDuration,
                LastWatchedDate = videoProgress.LastWatchedDate,
                IsCompleted = videoProgress.IsCompleted
            };
        }

        public async Task<IEnumerable<VideoProgressDto>> GetVideoProgressByUserIdAsync(int userId)
        {
            var videosProgress = await _repository.GetByUserIdAsync(userId);
            return videosProgress.Select(vp => new VideoProgressDto
            {
                Id = vp.Id,
                UserId = vp.UserId,
                UserName = vp.User?.FirstName + " " + vp.User?.LastName ?? "Unknown",
                VideoId = vp.VideoId,
                VideoTitle = vp.Video?.Title ?? "Unknown",
                WatchedDuration = vp.WatchedDuration,
                LastWatchedDate = vp.LastWatchedDate,
                IsCompleted = vp.IsCompleted
            });
        }

        public async Task<IEnumerable<VideoProgressDto>> GetVideoProgressByVideoIdAsync(int videoId)
        {
            var videosProgress = await _repository.GetByVideoIdAsync(videoId);
            return videosProgress.Select(vp => new VideoProgressDto
            {
                Id = vp.Id,
                UserId = vp.UserId,
                UserName = vp.User?.FirstName + " " + vp.User?.LastName ?? "Unknown",
                VideoId = vp.VideoId,
                VideoTitle = vp.Video?.Title ?? "Unknown",
                WatchedDuration = vp.WatchedDuration,
                LastWatchedDate = vp.LastWatchedDate,
                IsCompleted = vp.IsCompleted
            });
        }

        public async Task<VideoProgressDto?> GetVideoProgressByUserAndVideoAsync(int userId, int videoId)
        {
            var videoProgress = await _repository.GetByUserAndVideoAsync(userId, videoId);
            if (videoProgress == null) return null;

            return new VideoProgressDto
            {
                Id = videoProgress.Id,
                UserId = videoProgress.UserId,
                UserName = videoProgress.User?.FirstName + " " + videoProgress.User?.LastName ?? "Unknown",
                VideoId = videoProgress.VideoId,
                VideoTitle = videoProgress.Video?.Title ?? "Unknown",
                WatchedDuration = videoProgress.WatchedDuration,
                LastWatchedDate = videoProgress.LastWatchedDate,
                IsCompleted = videoProgress.IsCompleted
            };
        }

        public async Task<VideoProgressDto> CreateVideoProgressAsync(CreateVideoProgressDto dto)
        {
            var videoProgress = new VideoProgress
            {
                UserId = dto.UserId,
                VideoId = dto.VideoId,
                WatchedDuration = dto.WatchedDuration,
                IsCompleted = dto.IsCompleted,
                LastWatchedDate = DateTime.UtcNow
            };

            var createdVideoProgress = await _repository.CreateAsync(videoProgress);
            var result = await _repository.GetByIdAsync(createdVideoProgress.Id);

            return new VideoProgressDto
            {
                Id = result!.Id,
                UserId = result.UserId,
                UserName = result.User?.FirstName + " " + result.User?.LastName ?? "Unknown",
                VideoId = result.VideoId,
                VideoTitle = result.Video?.Title ?? "Unknown",
                WatchedDuration = result.WatchedDuration,
                LastWatchedDate = result.LastWatchedDate,
                IsCompleted = result.IsCompleted
            };
        }

        public async Task<VideoProgressDto> UpdateVideoProgressAsync(UpdateVideoProgressDto dto)
        {
            var videoProgress = new VideoProgress
            {
                Id = dto.Id,
                UserId = dto.UserId,
                VideoId = dto.VideoId,
                WatchedDuration = dto.WatchedDuration,
                IsCompleted = dto.IsCompleted,
                LastWatchedDate = DateTime.UtcNow
            };

            var updatedVideoProgress = await _repository.UpdateAsync(videoProgress);
            var result = await _repository.GetByIdAsync(updatedVideoProgress.Id);

            return new VideoProgressDto
            {
                Id = result!.Id,
                UserId = result.UserId,
                UserName = result.User?.FirstName + " " + result.User?.LastName ?? "Unknown",
                VideoId = result.VideoId,
                VideoTitle = result.Video?.Title ?? "Unknown",
                WatchedDuration = result.WatchedDuration,
                LastWatchedDate = result.LastWatchedDate,
                IsCompleted = result.IsCompleted
            };
        }

        public async Task<bool> DeleteVideoProgressAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
