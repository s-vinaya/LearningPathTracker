using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
using AutoMapper;

namespace EMPBACKEND.Services
{
    public class VideoRequestService : IVideoRequestService
    {
        private readonly IVideoRequestRepository _repository;
        private readonly IMapper _mapper;

        public VideoRequestService(IVideoRequestRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VideoRequestDto>> GetAllVideoRequestsAsync()
        {
            var videoRequests = await _repository.GetAllAsync();
            return videoRequests.Select(vr => new VideoRequestDto
            {
                Id = vr.Id,
                UserId = vr.UserId,
                UserName = vr.User?.FirstName + " " + vr.User?.LastName ?? "Unknown",
                VideoTitle = vr.VideoTitle,
                RequestDescription = vr.RequestDescription,
                Status = vr.Status,
                RequestedDate = vr.RequestedDate
            });
        }

        public async Task<VideoRequestDto?> GetVideoRequestByIdAsync(int id)
        {
            var videoRequest = await _repository.GetByIdAsync(id);
            if (videoRequest == null) return null;

            return new VideoRequestDto
            {
                Id = videoRequest.Id,
                UserId = videoRequest.UserId,
                UserName = videoRequest.User?.FirstName + " " + videoRequest.User?.LastName ?? "Unknown",
                VideoTitle = videoRequest.VideoTitle,
                RequestDescription = videoRequest.RequestDescription,
                Status = videoRequest.Status,
                RequestedDate = videoRequest.RequestedDate
            };
        }

        public async Task<IEnumerable<VideoRequestDto>> GetVideoRequestsByUserIdAsync(int userId)
        {
            var videoRequests = await _repository.GetByUserIdAsync(userId);
            return videoRequests.Select(vr => new VideoRequestDto
            {
                Id = vr.Id,
                UserId = vr.UserId,
                UserName = vr.User?.FirstName + " " + vr.User?.LastName ?? "Unknown",
                VideoTitle = vr.VideoTitle,
                RequestDescription = vr.RequestDescription,
                Status = vr.Status,
                RequestedDate = vr.RequestedDate
            });
        }

        public async Task<IEnumerable<VideoRequestDto>> GetVideoRequestsByStatusAsync(string status)
        {
            var videoRequests = await _repository.GetByStatusAsync(status);
            return videoRequests.Select(vr => new VideoRequestDto
            {
                Id = vr.Id,
                UserId = vr.UserId,
                UserName = vr.User?.FirstName + " " + vr.User?.LastName ?? "Unknown",
                VideoTitle = vr.VideoTitle,
                RequestDescription = vr.RequestDescription,
                Status = vr.Status,
                RequestedDate = vr.RequestedDate
            });
        }

        public async Task<VideoRequestDto> CreateVideoRequestAsync(CreateVideoRequestDto dto)
        {
            var videoRequest = new VideoRequest
            {
                UserId = dto.UserId,
                VideoTitle = dto.VideoTitle,
                RequestDescription = dto.RequestDescription,
                Status = "Pending",
                RequestedDate = DateTime.UtcNow
            };

            var createdVideoRequest = await _repository.CreateAsync(videoRequest);
            var result = await _repository.GetByIdAsync(createdVideoRequest.Id);

            return new VideoRequestDto
            {
                Id = result!.Id,
                UserId = result.UserId,
                UserName = result.User?.FirstName + " " + result.User?.LastName ?? "Unknown",
                VideoTitle = result.VideoTitle,
                RequestDescription = result.RequestDescription,
                Status = result.Status,
                RequestedDate = result.RequestedDate
            };
        }

        public async Task<VideoRequestDto> UpdateVideoRequestAsync(UpdateVideoRequestDto dto)
        {
            var videoRequest = new VideoRequest
            {
                Id = dto.Id,
                UserId = dto.UserId,
                VideoTitle = dto.VideoTitle,
                RequestDescription = dto.RequestDescription,
                Status = dto.Status
            };

            var updatedVideoRequest = await _repository.UpdateAsync(videoRequest);
            var result = await _repository.GetByIdAsync(updatedVideoRequest.Id);

            return new VideoRequestDto
            {
                Id = result!.Id,
                UserId = result.UserId,
                UserName = result.User?.FirstName + " " + result.User?.LastName ?? "Unknown",
                VideoTitle = result.VideoTitle,
                RequestDescription = result.RequestDescription,
                Status = result.Status,
                RequestedDate = result.RequestedDate
            };
        }

        public async Task<VideoRequestDto> UpdateVideoRequestStatusAsync(int id, string status)
        {
            var existingRequest = await _repository.GetByIdAsync(id);
            if (existingRequest == null) throw new ArgumentException("VideoRequest not found");

            existingRequest.Status = status;
            var updatedVideoRequest = await _repository.UpdateAsync(existingRequest);
            var result = await _repository.GetByIdAsync(updatedVideoRequest.Id);

            return new VideoRequestDto
            {
                Id = result!.Id,
                UserId = result.UserId,
                UserName = result.User?.FirstName + " " + result.User?.LastName ?? "Unknown",
                VideoTitle = result.VideoTitle,
                RequestDescription = result.RequestDescription,
                Status = result.Status,
                RequestedDate = result.RequestedDate
            };
        }

        public async Task<bool> DeleteVideoRequestAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}