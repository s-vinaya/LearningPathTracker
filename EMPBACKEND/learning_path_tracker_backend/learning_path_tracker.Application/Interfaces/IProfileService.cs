using learning_path_tracker.Application.DTOs;

namespace learning_path_tracker.Application.Interfaces;

public interface IProfileService
{
    Task<ProfileDto?> GetProfileAsync(int userId);
    Task<ProfileDto?> GetProfileByEmailAsync(string email);
    Task<ProfileDto?> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task<bool> UploadProfileImageAsync(int userId, byte[] imageData);
}
