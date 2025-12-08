using learning_path_tracker.Application.DTOs.Notifications;
using learning_path_tracker.Application.DTOs.Statistics;

namespace learning_path_tracker.Application.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDto>> GetAllNotificationsAsync();
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto, int recipientCount);
    Task<NotificationStatisticsDto> GetStatisticsAsync();
    Task<bool> DeleteNotificationAsync(int id);
    Task<NotificationDto?> GetNotificationByIdAsync(int id);
}
