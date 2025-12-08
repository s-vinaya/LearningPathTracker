using learning_path_tracker.Application.DTOs.Dashboard;

namespace learning_path_tracker.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
    Task<EngagementDataDto> GetEngagementDataAsync(int days);
    Task<List<RecentActivityDto>> GetRecentActivitiesAsync();
}
