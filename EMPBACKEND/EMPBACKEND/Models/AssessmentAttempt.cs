using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class AssessmentAttempt
    {
        [Key]
        public int AttemptId { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        public int AssessmentId { get; set; }
        public Assessment Assessment { get; set; } = null!;
        
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public bool Passed { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        
        public string? Answers { get; set; } // JSON format
        public int TimeSpentMinutes { get; set; } = 0;
        public int AttemptNumber { get; set; } = 1;
    }
}