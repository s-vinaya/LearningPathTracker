namespace EMPBACKEND.DTOs
{
    public class VideoRequestDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string VideoTitle { get; set; } = string.Empty;
        public string RequestDescription { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
    }

    public class CreateVideoRequestDto
    {
        public int UserId { get; set; }
        public string VideoTitle { get; set; } = string.Empty;
        public string RequestDescription { get; set; } = string.Empty;
    }

    public class UpdateVideoRequestDto
    {
        public string Status { get; set; } = string.Empty;
    }
}