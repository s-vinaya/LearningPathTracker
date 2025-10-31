using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Models;
using AutoMapper;

namespace EMPBACKEND.Services
{
    public class AssessmentAttemptService : IAssessmentAttemptService
    {
        private readonly IAssessmentAttemptRepository _repository;
        private readonly IMapper _mapper;

        public AssessmentAttemptService(IAssessmentAttemptRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AssessmentAttemptDto>> GetAllAsync()
        {
            var attempts = await _repository.GetAllAsync();
            return attempts.Select(a => new AssessmentAttemptDto
            {
                AttemptId = a.AttemptId,
                UserId = a.UserId,
                UserName = a.User.Username,
                AssessmentId = a.AssessmentId,
                AssessmentName = a.Assessment.Title,
                Score = a.Score,
                MaxScore = a.MaxScore,
                Passed = a.Passed,
                SubmittedAt = a.SubmittedAt,
                Answers = a.Answers,
                TimeSpentMinutes = a.TimeSpentMinutes,
                AttemptNumber = a.AttemptNumber
            });
        }

        public async Task<AssessmentAttemptDto?> GetByIdAsync(int id)
        {
            var attempt = await _repository.GetByIdAsync(id);
            return attempt != null ? new AssessmentAttemptDto
            {
                AttemptId = attempt.AttemptId,
                UserId = attempt.UserId,
                UserName = attempt.User.Username,
                AssessmentId = attempt.AssessmentId,
                AssessmentName = attempt.Assessment.Title,
                Score = attempt.Score,
                MaxScore = attempt.MaxScore,
                Passed = attempt.Passed,
                SubmittedAt = attempt.SubmittedAt,
                Answers = attempt.Answers,
                TimeSpentMinutes = attempt.TimeSpentMinutes,
                AttemptNumber = attempt.AttemptNumber
            } : null;
        }

        public async Task<AssessmentAttemptDto> CreateAsync(CreateAssessmentAttemptDto attemptDto)
        {
            var attempt = new AssessmentAttempt
            {
                UserId = attemptDto.UserId,
                AssessmentId = attemptDto.AssessmentId,
                Score = attemptDto.Score,
                MaxScore = attemptDto.MaxScore,
                Answers = attemptDto.Answers,
                TimeSpentMinutes = attemptDto.TimeSpentMinutes,
                SubmittedAt = DateTime.UtcNow,
                Passed = attemptDto.Score >= (attemptDto.MaxScore * 0.7),
                AttemptNumber = 1
            };
            var created = await _repository.CreateAsync(attempt);
            return new AssessmentAttemptDto
            {
                AttemptId = created.AttemptId,
                UserId = created.UserId,
                AssessmentId = created.AssessmentId,
                Score = created.Score,
                MaxScore = created.MaxScore,
                Passed = created.Passed,
                SubmittedAt = created.SubmittedAt,
                Answers = created.Answers,
                TimeSpentMinutes = created.TimeSpentMinutes,
                AttemptNumber = created.AttemptNumber
            };
        }

        public async Task<AssessmentAttemptDto> UpdateAsync(int id, AssessmentAttemptDto attemptDto)
        {
            var attempt = new AssessmentAttempt
            {
                AttemptId = id,
                UserId = attemptDto.UserId,
                AssessmentId = attemptDto.AssessmentId,
                Score = attemptDto.Score,
                MaxScore = attemptDto.MaxScore,
                Passed = attemptDto.Passed,
                SubmittedAt = attemptDto.SubmittedAt,
                Answers = attemptDto.Answers,
                TimeSpentMinutes = attemptDto.TimeSpentMinutes,
                AttemptNumber = attemptDto.AttemptNumber
            };
            var updated = await _repository.UpdateAsync(attempt);
            return new AssessmentAttemptDto
            {
                AttemptId = updated.AttemptId,
                UserId = updated.UserId,
                AssessmentId = updated.AssessmentId,
                Score = updated.Score,
                MaxScore = updated.MaxScore,
                Passed = updated.Passed,
                SubmittedAt = updated.SubmittedAt,
                Answers = updated.Answers,
                TimeSpentMinutes = updated.TimeSpentMinutes,
                AttemptNumber = updated.AttemptNumber
            };
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<AssessmentAttemptDto>> GetByUserIdAsync(int userId)
        {
            var attempts = await _repository.GetByUserIdAsync(userId);
            return attempts.Select(a => new AssessmentAttemptDto
            {
                AttemptId = a.AttemptId,
                UserId = a.UserId,
                AssessmentId = a.AssessmentId,
                Score = a.Score,
                MaxScore = a.MaxScore,
                Passed = a.Passed,
                SubmittedAt = a.SubmittedAt,
                Answers = a.Answers,
                TimeSpentMinutes = a.TimeSpentMinutes,
                AttemptNumber = a.AttemptNumber
            });
        }

        public async Task<IEnumerable<AssessmentAttemptDto>> GetByAssessmentIdAsync(int assessmentId)
        {
            var attempts = await _repository.GetByAssessmentIdAsync(assessmentId);
            return attempts.Select(a => new AssessmentAttemptDto
            {
                AttemptId = a.AttemptId,
                UserId = a.UserId,
                AssessmentId = a.AssessmentId,
                Score = a.Score,
                MaxScore = a.MaxScore,
                Passed = a.Passed,
                SubmittedAt = a.SubmittedAt,
                Answers = a.Answers,
                TimeSpentMinutes = a.TimeSpentMinutes,
                AttemptNumber = a.AttemptNumber
            });
        }
    }
}