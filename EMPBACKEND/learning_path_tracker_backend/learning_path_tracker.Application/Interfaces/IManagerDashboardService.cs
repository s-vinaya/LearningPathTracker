using learning_path_tracker.Application.DTOs.Dashboard;

namespace learning_path_tracker.Application.Interfaces;

public interface IManagerDashboardService
{
    Task<ManagerDashboardDto> GetManagerDashboardAsync(int managerId);
    Task<List<UpcomingDeadlineDto>> GetUpcomingDeadlinesAsync(int managerId, int days);
}
