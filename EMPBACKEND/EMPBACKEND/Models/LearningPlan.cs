using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class LearningPlan
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        
        public int AssignedBy { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "Assigned"; // Assigned, InProgress, Completed, Overdue
    }
}