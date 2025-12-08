using learning_path_tracker.Application.DTOs.Notifications;
using learning_path_tracker.Application.DTOs.Statistics;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Data.Repositories;
using learning_path_tracker.Domain.Entities;

namespace learning_path_tracker.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<NotificationDto>> GetAllNotificationsAsync()
    {
        var notifications = await _notificationRepository.GetAllAsync();
        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type,
            CreatedAt = n.CreatedAt,
            IsRead = n.IsRead,
            Recipients = n.Recipients,
            RecipientCount = n.RecipientCount,
            DeliveredCount = n.DeliveredCount,
            OpenedCount = n.OpenedCount,
            ClickedCount = n.ClickedCount
        }).ToList();
    }

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto, int recipientCount)
    {
        var notification = new Notification
        {
            Title = dto.Title,
            Message = dto.Message,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30),
            IsRead = false,
            Recipients = dto.Recipients,
            RecipientCount = recipientCount
        };

        var created = await _notificationRepository.AddAsync(notification);

        return new NotificationDto
        {
            Id = created.Id,
            Title = created.Title,
            Message = created.Message,
            Type = created.Type,
            CreatedAt = created.CreatedAt,
            IsRead = created.IsRead,
            Recipients = created.Recipients,
            RecipientCount = created.RecipientCount,
            DeliveredCount = created.DeliveredCount,
            OpenedCount = created.OpenedCount,
            ClickedCount = created.ClickedCount
        };
    }

    public async Task<NotificationStatisticsDto> GetStatisticsAsync()
    {
        var notifications = await _notificationRepository.GetAllAsync();
        var totalRecipients = notifications.Sum(n => n.RecipientCount);
        var totalDelivered = notifications.Sum(n => n.DeliveredCount);
        
        return new NotificationStatisticsDto
        {
            TotalSent = totalRecipients,
            DeliveryRate = totalRecipients > 0 ? Math.Round((double)totalDelivered / totalRecipients * 100, 0).ToString() : "0"
        };
    }

    public async Task<bool> DeleteNotificationAsync(int id)
    {
        return await _notificationRepository.DeleteAsync(id);
    }

    public async Task<NotificationDto?> GetNotificationByIdAsync(int id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null) return null;
        
        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead,
            Recipients = notification.Recipients,
            RecipientCount = notification.RecipientCount,
            DeliveredCount = notification.DeliveredCount,
            OpenedCount = notification.OpenedCount,
            ClickedCount = notification.ClickedCount
        };
    }
}
