using learning_path_tracker.Application.DTOs;

namespace learning_path_tracker.Application.Interfaces;

public interface IPlatformSettingsService
{
    Task<PlatformSettingsDto> GetSettingsAsync();
    Task<PlatformSettingsDto> UpdateSettingsAsync(PlatformSettingsDto dto);
}
