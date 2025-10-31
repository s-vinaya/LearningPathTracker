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
    public class LearningPathService : ILearningPathService
    {
        private readonly ILearningPathRepository _repository;
<<<<<<< Updated upstream

        public LearningPathService(ILearningPathRepository repository)
        {
            _repository = repository;
=======
        private readonly IMapper _mapper;

        public LearningPathService(ILearningPathRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
>>>>>>> Stashed changes
        }

        public async Task<IEnumerable<LearningPathDto>> GetAllAsync()
        {
            var learningPaths = await _repository.GetAllAsync();
            return learningPaths.Select(MapToDto);
        }

        public async Task<LearningPathDto?> GetByIdAsync(int id)
        {
            var learningPath = await _repository.GetByIdAsync(id);
            return learningPath != null ? MapToDto(learningPath) : null;
        }

        public async Task<LearningPathDto> CreateAsync(CreateLearningPathDto createDto)
        {
            var learningPath = new LearningPath
            {
                Title = createDto.Title,
                Description = createDto.Description,
                CreatedBy = createDto.CreatedBy,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(learningPath);
            var result = await _repository.GetByIdAsync(created.Id);
            return MapToDto(result!);
        }

        public async Task<LearningPathDto> UpdateAsync(int id, UpdateLearningPathDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) throw new ArgumentException("Learning path not found");

            existing.Title = updateDto.Title;
            existing.Description = updateDto.Description;
            existing.IsActive = updateDto.IsActive;
            existing.UpdatedDate = DateTime.UtcNow;

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

        public async Task<LearningPathCourseDto> AddCourseToPathAsync(int pathId, CreateLearningPathCourseDto courseDto)
        {
            // This would require a LearningPathCourse repository to implement properly
            // For now, return a placeholder DTO
            return await Task.FromResult(new LearningPathCourseDto
            {
                Id = 1,
                LearningPathId = pathId,
                CourseId = courseDto.CourseId,
                CourseName = "Sample Course",
                Order = courseDto.Order,
                IsRequired = courseDto.IsRequired
            });
        }

        public async Task<bool> RemoveCourseFromPathAsync(int pathId, int courseId)
        {
            // This would require a LearningPathCourse repository to implement properly
            // For now, return true as placeholder
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<LearningPathCourseDto>> GetLearningPathCoursesAsync(int pathId)
        {
            var learningPath = await _repository.GetByIdAsync(pathId);
            if (learningPath?.LearningPathCourses == null)
                return new List<LearningPathCourseDto>();
            
            return learningPath.LearningPathCourses.Select(lpc => new LearningPathCourseDto
            {
                Id = lpc.Id,
                LearningPathId = lpc.LearningPathId,
                CourseId = lpc.CourseId,
                CourseName = lpc.Course?.Title ?? string.Empty,
                Order = lpc.Order,
                IsRequired = lpc.IsRequired
            });
        }

        private static LearningPathDto MapToDto(LearningPath learningPath)
        {
            return new LearningPathDto
            {
                Id = learningPath.Id,
                Title = learningPath.Title,
                Description = learningPath.Description,
                CreatedBy = learningPath.CreatedBy,
                CreatedDate = learningPath.CreatedDate,
                UpdatedDate = learningPath.UpdatedDate,
                IsActive = learningPath.IsActive
            };
        }
    }
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
