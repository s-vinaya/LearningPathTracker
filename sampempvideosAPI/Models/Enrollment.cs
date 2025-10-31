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
        
        public decimal Progress { get; set; } = 0; // 0-100
        
        [StringLength(50)]
        public string Status { get; set; } = "InProgress"; // InProgress, Completed, Dropped
    }
}