using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class Course
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        
        public int Duration { get; set; } // in minutes
        
        [StringLength(500)]
        public string? VideoUrl { get; set; } // YouTube or other video URL
        
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        
      
        public ICollection<Video> Videos { get; set; } = new List<Video>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
        public ICollection<LearningPlan> LearningPlans { get; set; } = new List<LearningPlan>();
    }
}