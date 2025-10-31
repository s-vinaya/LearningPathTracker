using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Models;
using AutoMapper;

namespace EMPBACKEND.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IMapper _mapper;

        public VideoService(IVideoRepository videoRepository, IMapper mapper)
        {
            _videoRepository = videoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VideoDto>> GetAllVideoDtosAsync()
        {
            var videos = await _videoRepository.GetAllAsync();
            return videos.Select(v => new VideoDto
            {
                Id = v.Id,
                Title = v.Title,
                VideoUrl = v.VideoUrl,
                CourseId = v.CourseId,
                Duration = v.Duration,
                CreatedDate = v.CreatedDate
            });
        }

        public async Task<VideoDto?> GetVideoDtoByIdAsync(int id)
        {
            var video = await _videoRepository.GetByIdAsync(id);
            if (video == null) return null;

            return new VideoDto
            {
                Id = video.Id,
                Title = video.Title,
                VideoUrl = video.VideoUrl,
                CourseId = video.CourseId,
                Duration = video.Duration,
                CreatedDate = video.CreatedDate
            };
        }

        public async Task<IEnumerable<VideoDto>> GetVideoDtosByCourseAsync(int courseId)
        {
            var videos = await _videoRepository.GetByCourseIdAsync(courseId);
            return videos.Select(v => new VideoDto
            {
                Id = v.Id,
                Title = v.Title,
                VideoUrl = v.VideoUrl,
                CourseId = v.CourseId,
                Duration = v.Duration,
                CreatedDate = v.CreatedDate
            });
        }

        public async Task<VideoDto> CreateVideoFromDtoAsync(CreateVideoDto dto)
        {
            var video = new Video
            {
                Title = dto.Title,
                VideoUrl = dto.VideoUrl,
                CourseId = dto.CourseId,
                Duration = dto.Duration,
                CreatedDate = DateTime.UtcNow
            };

            var createdVideo = await _videoRepository.CreateAsync(video);
            return new VideoDto
            {
                Id = createdVideo.Id,
                Title = createdVideo.Title,
                VideoUrl = createdVideo.VideoUrl,
                CourseId = createdVideo.CourseId,
                Duration = createdVideo.Duration,
                CreatedDate = createdVideo.CreatedDate
            };
        }

        public async Task<VideoDto> UpdateVideoFromDtoAsync(int id, UpdateVideoDto dto)
        {
            var video = await _videoRepository.GetByIdAsync(id);
            if (video == null) throw new ArgumentException("Video not found");

            video.Title = dto.Title;
            video.VideoUrl = dto.VideoUrl;
            video.Duration = dto.Duration;

            var updatedVideo = await _videoRepository.UpdateAsync(video);
            return new VideoDto
            {
                Id = updatedVideo.Id,
                Title = updatedVideo.Title,
                VideoUrl = updatedVideo.VideoUrl,
                CourseId = updatedVideo.CourseId,
                Duration = updatedVideo.Duration,
                CreatedDate = updatedVideo.CreatedDate
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _videoRepository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _videoRepository.ExistsAsync(id);
        }
    }
}