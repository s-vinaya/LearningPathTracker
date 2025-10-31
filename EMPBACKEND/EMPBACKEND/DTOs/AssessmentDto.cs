using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.DTOs
{
    public class AssessmentDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Questions { get; set; } = string.Empty;
        public int PassingScore { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateAssessmentDto
    {
        [Required]
        public int CourseId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Questions { get; set; } = string.Empty;
        
        [Range(1, 100)]
        public int PassingScore { get; set; } = 70;
    }

    public class UpdateAssessmentDto
    {
        public string Title { get; set; } = string.Empty;
        public string Questions { get; set; } = string.Empty;
        public int PassingScore { get; set; }
    }
}