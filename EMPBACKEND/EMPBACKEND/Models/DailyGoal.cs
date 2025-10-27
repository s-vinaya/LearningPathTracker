using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class DailyGoal
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        [StringLength(100)]
        public string GoalType { get; set; } = string.Empty; // WatchTime, CompleteCourse, TakeAssessment
        
        public int TargetValue { get; set; }
        public int CurrentValue { get; set; } = 0;
        
        public DateTime Date { get; set; } = DateTime.Today;
        public bool IsCompleted { get; set; } = false;
    }
}