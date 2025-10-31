using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface IVideoService
    {
        Task<IEnumerable<VideoDto>> GetAllVideoDtosAsync();
        Task<VideoDto?> GetVideoDtoByIdAsync(int id);
        Task<IEnumerable<VideoDto>> GetVideoDtosByCourseAsync(int courseId);
        Task<VideoDto> CreateVideoFromDtoAsync(CreateVideoDto dto);
        Task<VideoDto> UpdateVideoFromDtoAsync(int id, UpdateVideoDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}