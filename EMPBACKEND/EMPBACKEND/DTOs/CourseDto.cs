using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string? VideoUrl { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateCourseDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        public int Duration { get; set; }
        
        public string? VideoUrl { get; set; }
        
        [Required]
        public int CreatedBy { get; set; }
    }

    public class UpdateCourseDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        public int Duration { get; set; }
        
        public string? VideoUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}