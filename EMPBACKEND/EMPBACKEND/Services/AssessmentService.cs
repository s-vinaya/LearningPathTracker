using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Models;

namespace EMPBACKEND.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly IAssessmentRepository _repository;

        public AssessmentService(IAssessmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AssessmentDto>> GetAllAsync()
        {
            var assessments = await _repository.GetAllAsync();
            return assessments.Select(a => new AssessmentDto
            {
                Id = a.Id,
                CourseId = a.CourseId,
                Title = a.Title,
                Questions = a.Questions,
                PassingScore = a.PassingScore,
                CreatedDate = a.CreatedDate
            });
        }

        public async Task<AssessmentDto?> GetByIdAsync(int id)
        {
            var assessment = await _repository.GetByIdAsync(id);
            return assessment != null ? new AssessmentDto
            {
                Id = assessment.Id,
                CourseId = assessment.CourseId,
                Title = assessment.Title,
                Questions = assessment.Questions,
                PassingScore = assessment.PassingScore,
                CreatedDate = assessment.CreatedDate
            } : null;
        }

        public async Task<AssessmentDto> CreateAsync(CreateAssessmentDto assessmentDto)
        {
            var assessment = new Assessment
            {
                CourseId = assessmentDto.CourseId,
                Title = assessmentDto.Title,
                Questions = assessmentDto.Questions,
                PassingScore = assessmentDto.PassingScore,
                CreatedDate = DateTime.UtcNow
            };
            var created = await _repository.CreateAsync(assessment);
            return new AssessmentDto
            {
                Id = created.Id,
                CourseId = created.CourseId,
                Title = created.Title,
                Questions = created.Questions,
                PassingScore = created.PassingScore,
                CreatedDate = created.CreatedDate
            };
        }

        public async Task<AssessmentDto> UpdateAsync(int id, UpdateAssessmentDto assessmentDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) throw new ArgumentException("Assessment not found");
            
            existing.Title = assessmentDto.Title;
            existing.Questions = assessmentDto.Questions;
            existing.PassingScore = assessmentDto.PassingScore;
            
            var updated = await _repository.UpdateAsync(existing);
            return new AssessmentDto
            {
                Id = updated.Id,
                CourseId = updated.CourseId,
                Title = updated.Title,
                Questions = updated.Questions,
                PassingScore = updated.PassingScore,
                CreatedDate = updated.CreatedDate
            };
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<AssessmentDto>> GetByCourseIdAsync(int courseId)
        {
            var assessments = await _repository.GetByCourseIdAsync(courseId);
            return assessments.Select(a => new AssessmentDto
            {
                Id = a.Id,
                CourseId = a.CourseId,
                Title = a.Title,
                Questions = a.Questions,
                PassingScore = a.PassingScore,
                CreatedDate = a.CreatedDate
            });
        }
    }
}