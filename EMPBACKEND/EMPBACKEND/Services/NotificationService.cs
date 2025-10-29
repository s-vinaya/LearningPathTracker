using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;

namespace EMPBACKEND.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<NotificationDto>> GetAllAsync()
        {
            var notifications = await _repository.GetAllAsync();
            return notifications.Select(MapToDto);
        }

        public async Task<NotificationDto?> GetByIdAsync(int id)
        {
            var notification = await _repository.GetByIdAsync(id);
            return notification != null ? MapToDto(notification) : null;
        }

        public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId)
        {
            var notifications = await _repository.GetByUserIdAsync(userId);
            return notifications.Select(MapToDto);
        }

        public async Task<NotificationDto> CreateAsync(CreateNotificationDto createDto)
        {
            var notification = new Notification
            {
                UserId = createDto.UserId,
                Title = createDto.Title,
                Message = createDto.Message,
                Type = createDto.Type,
                ActionUrl = createDto.ActionUrl,
                ActionText = createDto.ActionText,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(notification);
            var result = await _repository.GetByIdAsync(created.NotificationId);
            return MapToDto(result!);
        }

        public async Task<NotificationDto> UpdateAsync(int id, UpdateNotificationDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) throw new ArgumentException("Notification not found");

            existing.IsRead = updateDto.IsRead;
            if (updateDto.IsRead && existing.ReadAt == null)
            {
                existing.ReadAt = DateTime.UtcNow;
            }

            await _repository.UpdateAsync(existing);
            var updated = await _repository.GetByIdAsync(id);
            return MapToDto(updated!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadByUserIdAsync(int userId)
        {
            var notifications = await _repository.GetByUserIdAsync(userId);
            return notifications.Where(n => !n.IsRead).Select(MapToDto);
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            var updateDto = new UpdateNotificationDto { IsRead = true };
            await UpdateAsync(id, updateDto);
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var notifications = await _repository.GetByUserIdAsync(userId);
            var unreadNotifications = notifications.Where(n => !n.IsRead);
            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _repository.UpdateAsync(notification);
            }
            return true;
        }

        private static NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                UserName = notification.User?.Username ?? string.Empty,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt,
                ActionUrl = notification.ActionUrl,
                ActionText = notification.ActionText
            };
        }
    }
}