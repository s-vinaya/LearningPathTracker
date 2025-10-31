using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class CourseProgress
    {
        [Key]
        public int ProgressId { get; set; }
        
        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;
        
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        
        [StringLength(50)]
        public string Status { get; set; } = "NotStarted"; // NotStarted, InProgress, Completed
        
        public decimal PercentComplete { get; set; } = 0;
        public DateTime? LastAccessed { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int TimeSpentMinutes { get; set; } = 0;
    }
}