using EMPBACKEND.Interfaces;
using EMPBACKEND.Models;

namespace EMPBACKEND.Services
{
    public class VideoRequestService : IVideoRequestService
    {
        private readonly IVideoRequestRepository _requestRepository;

        public VideoRequestService(IVideoRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<IEnumerable<VideoRequest>> GetAllRequestsAsync()
        {
            return await _requestRepository.GetAllAsync();
        }

        public async Task<VideoRequest?> GetRequestByIdAsync(int id)
        {
            return await _requestRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<VideoRequest>> GetUserRequestsAsync(int userId)
        {
            return await _requestRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<VideoRequest>> GetPendingRequestsAsync()
        {
            return await _requestRepository.GetByStatusAsync("Pending");
        }

        public async Task<VideoRequest> CreateRequestAsync(VideoRequest videoRequest)
        {
            videoRequest.RequestedDate = DateTime.UtcNow;
            videoRequest.Status = "Pending";
            return await _requestRepository.CreateAsync(videoRequest);
        }

        public async Task<VideoRequest> ApproveRequestAsync(int id)
        {
            var request = await _requestRepository.GetByIdAsync(id);
            if (request == null)
                throw new ArgumentException("Video request not found");

            request.Status = "Approved";
            return await _requestRepository.UpdateAsync(request);
        }

        public async Task<VideoRequest> RejectRequestAsync(int id)
        {
            var request = await _requestRepository.GetByIdAsync(id);
            if (request == null)
                throw new ArgumentException("Video request not found");

            request.Status = "Rejected";
            return await _requestRepository.UpdateAsync(request);
        }
    }
}