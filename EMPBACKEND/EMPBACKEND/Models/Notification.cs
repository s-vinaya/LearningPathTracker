using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Type { get; set; } = "Info"; // Info, Warning, Success, Error
        
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }
        
        public string? ActionUrl { get; set; }
        public string? ActionText { get; set; }
    }
}