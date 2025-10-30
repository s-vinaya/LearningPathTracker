namespace EMPBACKEND.DTOs
{
    public class CourseProgressDto
    {
        public int ProgressId { get; set; }
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal PercentComplete { get; set; }
        public DateTime? LastAccessed { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int TimeSpentMinutes { get; set; }
    }

    public class UpdateCourseProgressDto
    {
        public string Status { get; set; } = string.Empty;
        public decimal PercentComplete { get; set; }
        public int TimeSpentMinutes { get; set; }
    }
}