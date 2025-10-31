using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface IAssessmentService
    {
        Task<IEnumerable<AssessmentDto>> GetAllAsync();
        Task<AssessmentDto?> GetByIdAsync(int id);
        Task<AssessmentDto> CreateAsync(CreateAssessmentDto assessmentDto);
        Task<AssessmentDto> UpdateAsync(int id, UpdateAssessmentDto assessmentDto);
        Task DeleteAsync(int id);
        Task<IEnumerable<AssessmentDto>> GetByCourseIdAsync(int courseId);
    }
}