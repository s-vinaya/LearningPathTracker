namespace EMPBACKEND.DTOs
{
    public class VideoProgressDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int VideoId { get; set; }
        public string VideoTitle { get; set; } = string.Empty;
        public int WatchedDuration { get; set; }
        public DateTime LastWatchedDate { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class CreateVideoProgressDto
    {
        public int UserId { get; set; }
        public int VideoId { get; set; }
        public int WatchedDuration { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class UpdateVideoProgressDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VideoId { get; set; }
        public int WatchedDuration { get; set; }
        public bool IsCompleted { get; set; }
    }
}