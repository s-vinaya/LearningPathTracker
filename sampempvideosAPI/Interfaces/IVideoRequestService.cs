using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces
{
    public interface IVideoRequestService
    {
        Task<IEnumerable<VideoRequest>> GetAllRequestsAsync();
        Task<VideoRequest?> GetRequestByIdAsync(int id);
        Task<IEnumerable<VideoRequest>> GetUserRequestsAsync(int userId);
        Task<IEnumerable<VideoRequest>> GetPendingRequestsAsync();
        Task<VideoRequest> CreateRequestAsync(VideoRequest videoRequest);
        Task<VideoRequest> ApproveRequestAsync(int id);
        Task<VideoRequest> RejectRequestAsync(int id);
    }
}