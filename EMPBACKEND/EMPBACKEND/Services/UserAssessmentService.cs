using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
<<<<<<< Updated upstream
=======
using AutoMapper;
>>>>>>> Stashed changes

namespace EMPBACKEND.Services
{
    public class UserAssessmentService : IUserAssessmentService
    {
        private readonly IUserAssessmentRepository _repository;
<<<<<<< Updated upstream

        public UserAssessmentService(IUserAssessmentRepository repository)
        {
            _repository = repository;
=======
        private readonly IMapper _mapper;

        public UserAssessmentService(IUserAssessmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
>>>>>>> Stashed changes
        }

        public async Task<IEnumerable<UserAssessmentDto>> GetAllUserAssessmentsAsync()
        {
            var userAssessments = await _repository.GetAllAsync();
            return userAssessments.Select(ua => new UserAssessmentDto
            {
                Id = ua.Id,
                UserId = ua.UserId,
                UserName = ua.User?.FirstName + " " + ua.User?.LastName ?? "Unknown",
                AssessmentId = ua.AssessmentId,
                AssessmentTitle = ua.Assessment?.Title ?? "Unknown",
                Score = ua.Score,
                AttemptDate = ua.AttemptDate,
                IsPassed = ua.IsPassed
            });
        }

        public async Task<UserAssessmentDto?> GetUserAssessmentByIdAsync(int id)
        {
            var userAssessment = await _repository.GetByIdAsync(id);
            if (userAssessment == null) return null;

            return new UserAssessmentDto
            {
                Id = userAssessment.Id,
                UserId = userAssessment.UserId,
                UserName = userAssessment.User?.FirstName + " " + userAssessment.User?.LastName ?? "Unknown",
                AssessmentId = userAssessment.AssessmentId,
                AssessmentTitle = userAssessment.Assessment?.Title ?? "Unknown",
                Score = userAssessment.Score,
                AttemptDate = userAssessment.AttemptDate,
                IsPassed = userAssessment.IsPassed
            };
        }

        public async Task<IEnumerable<UserAssessmentDto>> GetUserAssessmentsByUserIdAsync(int userId)
        {
            var userAssessments = await _repository.GetByUserIdAsync(userId);
            return userAssessments.Select(ua => new UserAssessmentDto
            {
                Id = ua.Id,
                UserId = ua.UserId,
                UserName = ua.User?.FirstName + " " + ua.User?.LastName ?? "Unknown",
                AssessmentId = ua.AssessmentId,
                AssessmentTitle = ua.Assessment?.Title ?? "Unknown",
                Score = ua.Score,
                AttemptDate = ua.AttemptDate,
                IsPassed = ua.IsPassed
            });
        }

        public async Task<IEnumerable<UserAssessmentDto>> GetUserAssessmentsByAssessmentIdAsync(int assessmentId)
        {
            var userAssessments = await _repository.GetByAssessmentIdAsync(assessmentId);
            return userAssessments.Select(ua => new UserAssessmentDto
            {
                Id = ua.Id,
                UserId = ua.UserId,
                UserName = ua.User?.FirstName + " " + ua.User?.LastName ?? "Unknown",
                AssessmentId = ua.AssessmentId,
                AssessmentTitle = ua.Assessment?.Title ?? "Unknown",
                Score = ua.Score,
                AttemptDate = ua.AttemptDate,
                IsPassed = ua.IsPassed
            });
        }

        public async Task<UserAssessmentDto?> GetUserAssessmentByUserAndAssessmentAsync(int userId, int assessmentId)
        {
            var userAssessment = await _repository.GetByUserAndAssessmentAsync(userId, assessmentId);
            if (userAssessment == null) return null;

            return new UserAssessmentDto
            {
                Id = userAssessment.Id,
                UserId = userAssessment.UserId,
                UserName = userAssessment.User?.FirstName + " " + userAssessment.User?.LastName ?? "Unknown",
                AssessmentId = userAssessment.AssessmentId,
                AssessmentTitle = userAssessment.Assessment?.Title ?? "Unknown",
                Score = userAssessment.Score,
                AttemptDate = userAssessment.AttemptDate,
                IsPassed = userAssessment.IsPassed
            };
        }

        public async Task<UserAssessmentDto> CreateUserAssessmentAsync(CreateUserAssessmentDto dto)
        {
            var userAssessment = new UserAssessment
            {
                UserId = dto.UserId,
                AssessmentId = dto.AssessmentId,
                Score = dto.Score,
                IsPassed = dto.IsPassed,
                AttemptDate = DateTime.UtcNow
            };

            var createdUserAssessment = await _repository.CreateAsync(userAssessment);
            var result = await _repository.GetByIdAsync(createdUserAssessment.Id);

            return new UserAssessmentDto
            {
                Id = result!.Id,
                UserId = result.UserId,
                UserName = result.User?.FirstName + " " + result.User?.LastName ?? "Unknown",
                AssessmentId = result.AssessmentId,
                AssessmentTitle = result.Assessment?.Title ?? "Unknown",
                Score = result.Score,
                AttemptDate = result.AttemptDate,
                IsPassed = result.IsPassed
            };
        }

        public async Task<UserAssessmentDto> UpdateUserAssessmentAsync(UpdateUserAssessmentDto dto)
        {
            var userAssessment = new UserAssessment
            {
                Id = dto.Id,
                UserId = dto.UserId,
                AssessmentId = dto.AssessmentId,
                Score = dto.Score,
                IsPassed = dto.IsPassed,
                AttemptDate = DateTime.UtcNow
            };

            var updatedUserAssessment = await _repository.UpdateAsync(userAssessment);
            var result = await _repository.GetByIdAsync(updatedUserAssessment.Id);

            return new UserAssessmentDto
            {
                Id = result!.Id,
                UserId = result.UserId,
                UserName = result.User?.FirstName + " " + result.User?.LastName ?? "Unknown",
                AssessmentId = result.AssessmentId,
                AssessmentTitle = result.Assessment?.Title ?? "Unknown",
                Score = result.Score,
                AttemptDate = result.AttemptDate,
                IsPassed = result.IsPassed
            };
        }

        public async Task<bool> DeleteUserAssessmentAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
