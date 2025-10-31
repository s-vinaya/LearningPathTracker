using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface IVideoProgressService
    {
        Task<IEnumerable<VideoProgressDto>> GetAllVideoProgressAsync();
        Task<VideoProgressDto?> GetVideoProgressByIdAsync(int id);
        Task<IEnumerable<VideoProgressDto>> GetVideoProgressByUserIdAsync(int userId);
        Task<IEnumerable<VideoProgressDto>> GetVideoProgressByVideoIdAsync(int videoId);
        Task<VideoProgressDto?> GetVideoProgressByUserAndVideoAsync(int userId, int videoId);
        Task<VideoProgressDto> CreateVideoProgressAsync(CreateVideoProgressDto dto);
        Task<VideoProgressDto> UpdateVideoProgressAsync(UpdateVideoProgressDto dto);
        Task<bool> DeleteVideoProgressAsync(int id);
    }
}