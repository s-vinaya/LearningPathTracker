using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Models;
using AutoMapper;

namespace EMPBACKEND.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly IAssessmentRepository _repository;
        private readonly IMapper _mapper;

        public AssessmentService(IAssessmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AssessmentDto>> GetAllAsync()
        {
            var assessments = await _repository.GetAllAsync();
<<<<<<< Updated upstream
            return assessments.Select(a => new AssessmentDto
            {
                Id = a.Id,
                CourseId = a.CourseId,
                CourseName = a.Course.Title,
                Title = a.Title,
                Questions = a.Questions,
                PassingScore = a.PassingScore,
                CreatedDate = a.CreatedDate
            });
=======
            return _mapper.Map<IEnumerable<AssessmentDto>>(assessments);
>>>>>>> Stashed changes
        }

        public async Task<AssessmentDto?> GetByIdAsync(int id)
        {
            var assessment = await _repository.GetByIdAsync(id);
<<<<<<< Updated upstream
            return assessment != null ? new AssessmentDto
            {
                Id = assessment.Id,
                CourseId = assessment.CourseId,
                CourseName = assessment.Course.Title,
                Title = assessment.Title,
                Questions = assessment.Questions,
                PassingScore = assessment.PassingScore,
                CreatedDate = assessment.CreatedDate
            } : null;
=======
            return assessment != null ? _mapper.Map<AssessmentDto>(assessment) : null;
>>>>>>> Stashed changes
        }

        public async Task<AssessmentDto> CreateAsync(CreateAssessmentDto assessmentDto)
        {
            var assessment = _mapper.Map<Assessment>(assessmentDto);
            var created = await _repository.CreateAsync(assessment);
            return _mapper.Map<AssessmentDto>(created);
        }

        public async Task<AssessmentDto> UpdateAsync(int id, UpdateAssessmentDto assessmentDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) throw new ArgumentException("Assessment not found");
            
            _mapper.Map(assessmentDto, existing);
            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<AssessmentDto>(updated);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<AssessmentDto>> GetByCourseIdAsync(int courseId)
        {
            var assessments = await _repository.GetByCourseIdAsync(courseId);
<<<<<<< Updated upstream
            return assessments.Select(a => new AssessmentDto
            {
                Id = a.Id,
                CourseId = a.CourseId,
                CourseName = a.Course.Title,
                Title = a.Title,
                Questions = a.Questions,
                PassingScore = a.PassingScore,
                CreatedDate = a.CreatedDate
            });
=======
            return _mapper.Map<IEnumerable<AssessmentDto>>(assessments);
>>>>>>> Stashed changes
        }
    }
}
