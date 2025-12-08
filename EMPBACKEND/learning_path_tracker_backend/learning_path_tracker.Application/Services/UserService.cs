using learning_path_tracker.Application.DTOs.Users;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Data.Repositories;
using learning_path_tracker.Domain.Entities;

namespace learning_path_tracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role,
            Department = u.Department,
            CreatedAt = u.CreatedAt,
            LastLogin = u.LastLogin,
            IsActive = u.IsActive,
            IsApproved = u.IsApproved,
            ProfileImageBase64 = u.ProfileImage != null ? Convert.ToBase64String(u.ProfileImage) : null
        }).ToList();
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Department = user.Department,
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin,
            IsActive = user.IsActive,
            IsApproved = user.IsApproved,
            ProfileImageBase64 = user.ProfileImage != null ? Convert.ToBase64String(user.ProfileImage) : null
        };
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = dto.Role,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _userRepository.AddAsync(user);

        return new UserDto
        {
            Id = created.Id,
            Name = created.Name,
            Email = created.Email,
            Role = created.Role,
            CreatedAt = created.CreatedAt,
            IsActive = created.IsActive
        };
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.Role = dto.Role;
        user.IsActive = dto.IsActive;
        if (dto.Department != null) user.Department = dto.Department;
        if (dto.ManagerId.HasValue) user.ManagerId = dto.ManagerId;

        var updated = await _userRepository.UpdateAsync(user);

        return new UserDto
        {
            Id = updated.Id,
            Name = updated.Name,
            Email = updated.Email,
            Role = updated.Role,
            CreatedAt = updated.CreatedAt,
            IsActive = updated.IsActive
        };
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;

        await _userRepository.DeleteAsync(id);
        return true;
    }
}
