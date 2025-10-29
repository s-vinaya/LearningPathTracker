using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public string Salt { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Employee"; // Admin, Manager, Employee
        
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        
        public bool IsActive { get; set; } = true;
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<UserAssessment> UserAssessments { get; set; } = new List<UserAssessment>();
        public ICollection<DailyGoal> DailyGoals { get; set; } = new List<DailyGoal>();
        public ICollection<LearningPlan> LearningPlans { get; set; } = new List<LearningPlan>();
        public ICollection<VideoRequest> VideoRequests { get; set; } = new List<VideoRequest>();
        public ICollection<VideoProgress> VideoProgresses { get; set; } = new List<VideoProgress>();
    }
}