using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.DTOs
{
    public class LearningPlanDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int? LearningPathId { get; set; }
        public string LearningPathName { get; set; } = string.Empty;
        public int AssignedBy { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateLearningPlanDto
    {
        [Required]
        public int UserId { get; set; }
        
        public int? CourseId { get; set; }
        
        public int? LearningPathId { get; set; }
        
        [Required]
        public int AssignedBy { get; set; }
        
        [Required]
        public DateTime DueDate { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "Assigned";
    }

    public class UpdateLearningPlanDto
    {
        [Required]
        public DateTime DueDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;
    }
}