using learning_path_tracker.Application.DTOs.LearningPaths;
using learning_path_tracker.Application.DTOs.Statistics;

namespace learning_path_tracker.Application.Interfaces;

public interface ILearningPathService
{
    Task<List<LearningPathDto>> GetAllLearningPathsAsync();
    Task<LearningPathDto> CreateLearningPathAsync(CreateLearningPathDto dto);
    Task<LearningPathDto?> UpdateLearningPathAsync(int id, UpdateLearningPathDto dto);
    Task<bool> DeleteLearningPathAsync(int id);
    Task<bool> UpdateModulesAsync(int learningPathId, List<CreateModuleDto> modules);
    Task<ModuleDto> AddModuleAsync(int learningPathId, CreateModuleDto dto);
    Task<ModuleDto?> UpdateModuleAsync(int moduleId, UpdateModuleDto dto);
    Task<bool> DeleteModuleAsync(int moduleId);
    Task<LearningPathStatisticsDto> GetStatisticsAsync();
    Task<List<object>> GetLearningPathsWithCoursesAsync();
    Task<List<object>> GetEmployeeLearningPathsAsync(int employeeId);
    Task<List<object>> GetManagerLearningPathsAsync(int managerId);
    
    // New methods for course management
    Task<object> CreateLearningPathWithCoursesAsync(CreateLearningPathWithCoursesDto dto);
    Task<bool> AddCoursesToPathAsync(AddCoursesToPathDto dto);
    Task<bool> UpdateCourseOrderAsync(UpdateCourseOrderDto dto);
    Task<bool> RemoveCourseFromPathAsync(int learningPathId, int courseId);
    Task<List<object>> GetAvailableCoursesAsync();
}
