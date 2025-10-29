using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.DTOs
{
    public class LearningPathCourseDto
    {
        public int Id { get; set; }
        public int LearningPathId { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsRequired { get; set; }
    }

    public class CreateLearningPathCourseDto
    {
        [Required]
        public int CourseId { get; set; }
        
        [Required]
        public int Order { get; set; }
        
        public bool IsRequired { get; set; } = true;
    }
}