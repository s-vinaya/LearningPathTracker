namespace EMPBACKEND.Models
{
    public class UserAssessment
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        public int AssessmentId { get; set; }
        public Assessment Assessment { get; set; } = null!;
        
        public int Score { get; set; }
        public DateTime AttemptDate { get; set; } = DateTime.UtcNow;
        public bool IsPassed { get; set; }
    }
}