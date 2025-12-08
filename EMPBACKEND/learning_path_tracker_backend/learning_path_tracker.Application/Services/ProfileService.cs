using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Data.Repositories;

namespace learning_path_tracker.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;

    public ProfileService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ProfileDto?> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        return new ProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Department = user.Department,
            JobTitle = user.JobTitle,
            Role = user.Role,
            ProfileImageBase64 = user.ProfileImage != null ? Convert.ToBase64String(user.ProfileImage) : null,
            LastLogin = user.LastLogin
        };
    }

    public async Task<ProfileDto?> GetProfileByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return null;

        return new ProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Department = user.Department,
            JobTitle = user.JobTitle,
            Role = user.Role,
            ProfileImageBase64 = user.ProfileImage != null ? Convert.ToBase64String(user.ProfileImage) : null,
            LastLogin = user.LastLogin
        };
    }

    public async Task<ProfileDto?> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.Department = dto.Department;
        user.JobTitle = dto.JobTitle;

        await _userRepository.UpdateAsync(user);

        return new ProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Department = user.Department,
            JobTitle = user.JobTitle,
            Role = user.Role,
            ProfileImageBase64 = user.ProfileImage != null ? Convert.ToBase64String(user.ProfileImage) : null,
            LastLogin = user.LastLogin
        };
    }

    public async Task<bool> UploadProfileImageAsync(int userId, byte[] imageData)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        user.ProfileImage = imageData;
        await _userRepository.UpdateAsync(user);
        return true;
    }
}
