using learning_path_tracker.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/track")]
public class NotificationTrackingController : ControllerBase
{
    private readonly AppDbContext _context;

    public NotificationTrackingController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("open/{notificationId}/{userId}")]
    public async Task<IActionResult> TrackOpen(int notificationId, int userId)
    {
        var tracking = await _context.NotificationTrackings
            .FirstOrDefaultAsync(t => t.NotificationId == notificationId && t.UserId == userId);

        if (tracking != null && !tracking.IsOpened)
        {
            tracking.IsOpened = true;
            tracking.OpenedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30);
            
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.OpenedCount++;
            }
            
            await _context.SaveChangesAsync();
        }

        return File(Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7"), "image/gif");
    }

    [HttpGet("click/{notificationId}/{userId}")]
    public async Task<IActionResult> TrackClick(int notificationId, int userId, [FromQuery] string url)
    {
        var tracking = await _context.NotificationTrackings
            .FirstOrDefaultAsync(t => t.NotificationId == notificationId && t.UserId == userId);

        if (tracking != null && !tracking.IsClicked)
        {
            tracking.IsClicked = true;
            tracking.ClickedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30);
            
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.ClickedCount++;
            }
            
            await _context.SaveChangesAsync();
        }

        return Redirect(url);
    }
}
