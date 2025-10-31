using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        
        public DateTime EnrolledDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletionDate { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "Enrolled"; // Enrolled, InProgress, Completed, Dropped
        
        public decimal Progress { get; set; } = 0;
        
        // Navigation properties
        public ICollection<CourseProgress> CourseProgresses { get; set; } = new List<CourseProgress>();
    }
}