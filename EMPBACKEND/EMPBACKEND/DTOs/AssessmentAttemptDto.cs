namespace EMPBACKEND.DTOs
{
    public class AssessmentAttemptDto
    {
        public int AttemptId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int AssessmentId { get; set; }
        public string AssessmentName { get; set; } = string.Empty;
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public bool Passed { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string? Answers { get; set; }
        public int TimeSpentMinutes { get; set; }
        public int AttemptNumber { get; set; }
    }

    public class CreateAssessmentAttemptDto
    {
        public int UserId { get; set; }
        public int AssessmentId { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public string? Answers { get; set; }
        public int TimeSpentMinutes { get; set; }
    }
}