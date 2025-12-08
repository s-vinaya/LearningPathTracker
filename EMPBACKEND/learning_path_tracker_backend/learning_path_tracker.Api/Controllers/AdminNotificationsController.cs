using learning_path_tracker.Application.DTOs.Notifications;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Application.Constants;
using learning_path_tracker.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Api.Controllers;
// TODO: Use constants instead of magic string, move them to a constant file and refer them 
// TODO: add file logging 
[ApiController]
[Route("api/admin/notifications")]
public class AdminNotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public AdminNotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var notifications = await _notificationService.GetAllNotificationsAsync();
        return Ok(notifications);
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var stats = await _notificationService.GetStatisticsAsync();
        return Ok(stats);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(id);
        return notification == null ? NotFound() : Ok(notification);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationDto dto, [FromServices] learning_path_tracker.Application.Services.EmailService emailService, [FromServices] IUserRepository userRepository, [FromServices] IServiceScopeFactory scopeFactory)
    {
        var allUsers = await userRepository.GetAllAsync();
        var users = dto.Recipients.ToLower() switch
        {
            "employees" => allUsers.Where(u => u.Role.Equals("Employee", StringComparison.OrdinalIgnoreCase)).ToList(),
            "managers" => allUsers.Where(u => u.Role.Equals("Manager", StringComparison.OrdinalIgnoreCase)).ToList(),
            "admins" => allUsers.Where(u => u.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase)).ToList(),
            _ => allUsers
        };
        
        var notification = await _notificationService.CreateNotificationAsync(dto, users.Count);
        
        using (var scope = scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<learning_path_tracker.Database.AppDbContext>();
            foreach (var user in users)
            {
                context.NotificationTrackings.Add(new learning_path_tracker.Domain.Entities.NotificationTracking
                {
                    NotificationId = notification.Id,
                    UserId = user.Id,
                    IsDelivered = false,
                    IsOpened = false,
                    IsClicked = false
                });
            }
            await context.SaveChangesAsync();
        }
        
        _ = Task.Run(async () =>
        {
            await Task.Delay(100);
            
            foreach (var user in users)
            {
                using var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<learning_path_tracker.Database.AppDbContext>();
                
                var tracking = await context.NotificationTrackings
                    .FirstOrDefaultAsync(t => t.NotificationId == notification.Id && t.UserId == user.Id);
                
                if (tracking == null) continue;
                
                try
                {
                    if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@"))
                    {
                        Console.WriteLine($"Invalid email for {user.Name}: {user.Email}");
                        continue;
                    }
                    
                    var emailBody = EmailConstants.GetNotificationBody(user.Name, dto.Title, dto.Message);
                    await emailService.SendEmailAsync(user.Email, dto.Title, emailBody);
                    
                    tracking.IsDelivered = true;
                    tracking.DeliveredAt = DateTime.UtcNow.AddHours(5).AddMinutes(30);
                    
                    var notif = await context.Notifications.FindAsync(notification.Id);
                    if (notif != null) notif.DeliveredCount++;
                    
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Email delivered to {user.Email}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email sending failed for {user.Email}: {ex.Message}");
                    tracking.IsDelivered = false;
                    await context.SaveChangesAsync();
                }
            }
        });
        
        return Ok(new { message = "Emails are being sent in the background", notification });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _notificationService.DeleteNotificationAsync(id);
        return result ? Ok(new { message = "Notification deleted successfully" }) : NotFound();
    }

    [HttpGet("{id}/delivery-status")]
    public async Task<IActionResult> GetDeliveryStatus(int id, [FromServices] learning_path_tracker.Database.AppDbContext context)
    {
        var trackings = await context.NotificationTrackings
            .Include(t => t.User)
            .Where(t => t.NotificationId == id)
            .Select(t => new
            {
                t.UserId,
                UserName = t.User.Name,
                UserEmail = t.User.Email,
                t.IsDelivered,
                t.IsOpened,
                t.IsClicked,
                t.DeliveredAt,
                t.OpenedAt,
                t.ClickedAt
            })
            .ToListAsync();
        
        return Ok(new
        {
            delivered = trackings.Where(t => t.IsDelivered).ToList(),
            failed = trackings.Where(t => !t.IsDelivered).ToList()
        });
    }

    [HttpPost("{id}/simulate-opens")]
    public async Task<IActionResult> SimulateOpens(int id, [FromServices] learning_path_tracker.Database.AppDbContext context)
    {
        var trackings = await context.NotificationTrackings
            .Where(t => t.NotificationId == id && t.IsDelivered && !t.IsOpened)
            .ToListAsync();
        
        var notification = await context.Notifications.FindAsync(id);
        if (notification == null) return NotFound();
        
        foreach (var tracking in trackings)
        {
            tracking.IsOpened = true;
            tracking.OpenedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30);
            notification.OpenedCount++;
        }
        
        await context.SaveChangesAsync();
        return Ok(new { message = $"Simulated {trackings.Count} opens" });
    }
}
