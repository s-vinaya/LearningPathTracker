using learning_path_tracker.Application.DTOs;

namespace learning_path_tracker.Application.Interfaces;

public interface IManagerAssignmentService
{
    Task<object> GetLearningPathsAsync();
    Task<object> GetTeamEmployeesAsync(int managerId);
    Task<bool> AssignLearningPathAsync(int managerId, AssignLearningPathRequestDto dto);
    Task<bool> BulkAssignLearningPathAsync(int managerId, BulkAssignDto dto);
    Task<bool> ReassignLearningPathAsync(int managerId, ReassignDto dto);
    Task<bool> RemoveAssignmentAsync(int managerId, int assignmentId);
}
