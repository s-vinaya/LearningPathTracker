namespace EMPBACKEND.DTOs
{
    public class VideoDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int Duration { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateVideoDto
    {
        public string Title { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int Duration { get; set; }
    }

    public class UpdateVideoDto
    {
        public string Title { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int Duration { get; set; }
    }
}