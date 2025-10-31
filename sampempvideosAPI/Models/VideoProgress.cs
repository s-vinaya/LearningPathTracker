using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMPBACKEND.Models
{
    public class VideoProgress
    {
        public int Id { get; set; }
        
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        [ForeignKey("Video")]
        public int VideoId { get; set; }
        public Video Video { get; set; } = null!;
        
        [Required]
        [Range(0, 86400)]
        public int WatchedDuration { get; set; } // in seconds
        
        [Required]
        public DateTime LastWatchedDate { get; set; } = DateTime.UtcNow;
        
        public bool IsCompleted { get; set; } = false;
    }
}