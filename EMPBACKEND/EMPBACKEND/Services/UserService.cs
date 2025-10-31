using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
using AutoMapper;

namespace EMPBACKEND.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<IEnumerable<UserDto>> GetByDepartmentAsync(int departmentId)
        {
            var users = await _userRepository.GetByDepartmentAsync(departmentId);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto createDto)
        {
            var user = _mapper.Map<User>(createDto);

            var createdUser = await _userRepository.CreateAsync(user);
            var result = await _userRepository.GetByIdAsync(createdUser.Id);
            return _mapper.Map<UserDto>(result!);
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new ArgumentException("User not found");

            _mapper.Map(updateDto, user);

            var updatedUser = await _userRepository.UpdateAsync(user);
            var result = await _userRepository.GetByIdAsync(updatedUser.Id);
            return _mapper.Map<UserDto>(result!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _userRepository.ExistsAsync(id);
        }


    }
}