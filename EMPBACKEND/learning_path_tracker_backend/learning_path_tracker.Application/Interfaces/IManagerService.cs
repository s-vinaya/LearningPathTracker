namespace learning_path_tracker.Application.Interfaces;

public interface IManagerService
{
    Task<object> GetDashboardStatsAsync(int managerId);
    Task<object> GetTeamMembersAsync(int managerId);
    Task<object> GetLearningPathsAsync();
    Task<object> GetReportsAsync(int managerId);
    Task<object> GetWeeklyHoursAsync(int managerId);
    Task<object> GetManagerProfileAsync(int managerId);
    Task<object> GetAssignmentsAsync(int managerId);
    Task<object> GetAllApprovalsAsync(int managerId);
    Task<object> GetPendingApprovalsAsync(int managerId);
    Task<bool> ApproveRequestAsync(int approvalId, int managerId, string? comments);
    Task<bool> RejectRequestAsync(int approvalId, int managerId, string? comments);
    Task<object?> GetEmployeeDetailsAsync(int employeeId, int managerId);
    Task<object?> GetEmployeeProgressAsync(int employeeId, int managerId);
    Task<bool> SendReminderAsync(int employeeId, int managerId);
    Task<bool> UpdateManagerProfileAsync(int managerId, learning_path_tracker.Application.DTOs.UpdateProfileDto dto);
    Task<bool> UpdateNotificationSettingsAsync(int managerId, learning_path_tracker.Application.DTOs.NotificationSettingsDto dto);
    Task<bool> UpdateTeamSettingsAsync(int managerId, learning_path_tracker.Application.DTOs.TeamSettingsDto dto);
    Task<learning_path_tracker.Application.DTOs.NotificationSettingsDto> GetNotificationSettingsAsync(int managerId);
    Task<learning_path_tracker.Application.DTOs.TeamSettingsDto> GetTeamSettingsAsync(int managerId);
    Task<bool> UpdateProfileImageAsync(int managerId, byte[] imageBytes);
    Task<List<object>> GetPendingCertificateRequestsAsync(int managerId);
    Task<object> ApproveCertificateRequestAsync(int requestId, int managerId);
    Task<object> RejectCertificateRequestAsync(int requestId, int managerId, string rejectionReason);
}
