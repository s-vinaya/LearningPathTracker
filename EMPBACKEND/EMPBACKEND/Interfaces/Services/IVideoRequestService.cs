using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface IVideoRequestService
    {
        Task<IEnumerable<VideoRequestDto>> GetAllVideoRequestsAsync();
        Task<VideoRequestDto?> GetVideoRequestByIdAsync(int id);
        Task<IEnumerable<VideoRequestDto>> GetVideoRequestsByUserIdAsync(int userId);
        Task<IEnumerable<VideoRequestDto>> GetVideoRequestsByStatusAsync(string status);
        Task<VideoRequestDto> CreateVideoRequestAsync(CreateVideoRequestDto dto);
        Task<VideoRequestDto> UpdateVideoRequestAsync(UpdateVideoRequestDto dto);
        Task<VideoRequestDto> UpdateVideoRequestStatusAsync(int id, string status);
        Task<bool> DeleteVideoRequestAsync(int id);
    }
}