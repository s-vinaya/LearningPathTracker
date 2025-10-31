using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class LearningPath
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public ICollection<LearningPathCourse> LearningPathCourses { get; set; } = new List<LearningPathCourse>();
        public ICollection<LearningPlan> LearningPlans { get; set; } = new List<LearningPlan>();
    }
    
    public class LearningPathCourse
    {
        public int Id { get; set; }
        
        public int LearningPathId { get; set; }
        public LearningPath LearningPath { get; set; } = null!;
        
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        
        public int Order { get; set; }
        public bool IsRequired { get; set; } = true;
    }
}