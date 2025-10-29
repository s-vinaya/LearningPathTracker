using EMPBACKEND.DTOs;

namespace EMPBACKEND.Interfaces.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetAllAsync();
        Task<NotificationDto?> GetByIdAsync(int id);
        Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId);
        Task<NotificationDto> CreateAsync(CreateNotificationDto dto);
        Task<NotificationDto> UpdateAsync(int id, UpdateNotificationDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<NotificationDto>> GetUnreadByUserIdAsync(int userId);
        Task<bool> MarkAsReadAsync(int id);
        Task<bool> MarkAllAsReadAsync(int userId);
    }
}