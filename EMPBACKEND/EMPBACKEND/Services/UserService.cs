using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;

namespace EMPBACKEND.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<IEnumerable<UserDto>> GetByDepartmentAsync(int departmentId)
        {
            var users = await _userRepository.GetByDepartmentAsync(departmentId);
            return users.Select(MapToDto);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto createDto)
        {
            var user = new User
            {
                Username = createDto.Username,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Email = createDto.Email,
                Password = createDto.Password,
                Salt = createDto.Salt,
                Role = createDto.Role,
                DepartmentId = createDto.DepartmentId,
                IsActive = true,
                IsApproved = false,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);
            var result = await _userRepository.GetByIdAsync(createdUser.Id);
            return MapToDto(result!);
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new ArgumentException("User not found");

            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.Email = updateDto.Email;
            user.Role = updateDto.Role;
            user.DepartmentId = updateDto.DepartmentId;
            user.IsActive = updateDto.IsActive;
            user.IsApproved = updateDto.IsApproved;
            user.UpdatedDate = DateTime.UtcNow;

            var updatedUser = await _userRepository.UpdateAsync(user);
            var result = await _userRepository.GetByIdAsync(updatedUser.Id);
            return MapToDto(result!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _userRepository.ExistsAsync(id);
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                DepartmentId = user.DepartmentId,
                DepartmentName = user.Department?.Name ?? string.Empty,
                IsActive = user.IsActive,
                IsApproved = user.IsApproved,
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdatedDate
            };
        }
    }
}