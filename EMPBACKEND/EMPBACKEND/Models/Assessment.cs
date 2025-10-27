using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class Assessment
    {
        public int Id { get; set; }
        
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Questions { get; set; } = string.Empty; // JSON format
        
        public int PassingScore { get; set; } = 70;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ICollection<UserAssessment> UserAssessments { get; set; } = new List<UserAssessment>();
    }
}