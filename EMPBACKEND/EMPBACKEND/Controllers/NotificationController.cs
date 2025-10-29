using Microsoft.AspNetCore.Mvc;
using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAll()
        {
            var notifications = await _service.GetAllAsync();
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDto>> GetById(int id)
        {
            var notification = await _service.GetByIdAsync(id);
            return notification == null ? NotFound() : Ok(notification);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetByUserId(int userId)
        {
            var notifications = await _service.GetByUserIdAsync(userId);
            return Ok(notifications);
        }

        [HttpGet("user/{userId}/unread")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUnreadByUserId(int userId)
        {
            var notifications = await _service.GetUnreadByUserIdAsync(userId);
            return Ok(notifications);
        }

        [HttpPost]
        public async Task<ActionResult<NotificationDto>> Create(CreateNotificationDto createDto)
        {
            var notification = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = notification.NotificationId }, notification);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<NotificationDto>> Update(int id, UpdateNotificationDto updateDto)
        {
            var notification = await _service.UpdateAsync(id, updateDto);
            return Ok(notification);
        }

        [HttpPut("{id}/read")]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            var result = await _service.MarkAsReadAsync(id);
            return result ? Ok() : NotFound();
        }

        [HttpPut("user/{userId}/read-all")]
        public async Task<ActionResult> MarkAllAsRead(int userId)
        {
            var result = await _service.MarkAllAsReadAsync(userId);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}