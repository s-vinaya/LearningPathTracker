using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.DTOs
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public DateTime EnrolledDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Progress { get; set; }
    }

    public class CreateEnrollmentDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int CourseId { get; set; }
    }

    public class UpdateEnrollmentDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
        
        [Range(0, 100)]
        public decimal Progress { get; set; }
        
        public DateTime? CompletionDate { get; set; }
    }
}