namespace EMPBACKEND.DTOs
{
    public class DailyGoalDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string GoalType { get; set; } = string.Empty;
        public int TargetValue { get; set; }
        public int CurrentValue { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class CreateDailyGoalDto
    {
        public int UserId { get; set; }
        public string GoalType { get; set; } = string.Empty;
        public int TargetValue { get; set; }
        public DateTime Date { get; set; }
    }

    public class UpdateDailyGoalDto
    {
        public string GoalType { get; set; } = string.Empty;
        public int TargetValue { get; set; }
        public int CurrentValue { get; set; }
        public bool IsCompleted { get; set; }
    }
}