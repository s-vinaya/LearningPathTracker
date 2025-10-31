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
    public class LearningPlanService : ILearningPlanService
    {
        private readonly ILearningPlanRepository _repository;
<<<<<<< Updated upstream

        public LearningPlanService(ILearningPlanRepository repository)
        {
            _repository = repository;
=======
        private readonly IMapper _mapper;

        public LearningPlanService(ILearningPlanRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
>>>>>>> Stashed changes
        }

        public async Task<IEnumerable<LearningPlanDto>> GetAllAsync()
        {
            var learningPlans = await _repository.GetAllAsync();
            return learningPlans.Select(MapToDto);
        }

        public async Task<LearningPlanDto?> GetByIdAsync(int id)
        {
            var learningPlan = await _repository.GetByIdAsync(id);
            return learningPlan != null ? MapToDto(learningPlan) : null;
        }

        public async Task<IEnumerable<LearningPlanDto>> GetByUserIdAsync(int userId)
        {
            var learningPlans = await _repository.GetByUserIdAsync(userId);
            return learningPlans.Select(MapToDto);
        }

        public async Task<LearningPlanDto> CreateAsync(CreateLearningPlanDto createDto)
        {
            var learningPlan = new LearningPlan
            {
                UserId = createDto.UserId,
                CourseId = createDto.CourseId,
                LearningPathId = createDto.LearningPathId,
                AssignedBy = createDto.AssignedBy,
                DueDate = createDto.DueDate,
                Status = createDto.Status,
                AssignedDate = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(learningPlan);
            var result = await _repository.GetByIdAsync(created.Id);
            return MapToDto(result!);
        }

        public async Task<LearningPlanDto> UpdateAsync(int id, UpdateLearningPlanDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) throw new ArgumentException("Learning plan not found");

            existing.DueDate = updateDto.DueDate;
            existing.Status = updateDto.Status;

            await _repository.UpdateAsync(existing);
            var updated = await _repository.GetByIdAsync(id);
            return MapToDto(updated!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        private static LearningPlanDto MapToDto(LearningPlan learningPlan)
        {
            return new LearningPlanDto
            {
                Id = learningPlan.Id,
                UserId = learningPlan.UserId,
                UserName = learningPlan.User?.Username ?? string.Empty,
                CourseId = learningPlan.CourseId,
                CourseName = learningPlan.Course?.Title ?? string.Empty,
                LearningPathId = learningPlan.LearningPathId,
                LearningPathName = learningPlan.LearningPath?.Title ?? string.Empty,
                AssignedBy = learningPlan.AssignedBy,
                AssignedDate = learningPlan.AssignedDate,
                DueDate = learningPlan.DueDate,
                Status = learningPlan.Status
            };
        }
    }
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
