namespace EMPBACKEND.Models
{
    public class VideoProgress
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        public int VideoId { get; set; }
        public Video Video { get; set; } = null!;
        
        public int WatchedDuration { get; set; } // in seconds
        public DateTime LastWatchedDate { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; } = false;
    }
}