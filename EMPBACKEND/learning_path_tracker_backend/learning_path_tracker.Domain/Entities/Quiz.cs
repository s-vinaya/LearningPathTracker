namespace learning_path_tracker.Domain.Entities;

public class Quiz
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? CourseId { get; set; }
    public int? LearningPathId { get; set; }
    public int PassingScore { get; set; }
    public int TimeLimit { get; set; }
    public bool IsActive { get; set; }
    public bool IsFinalQuiz { get; set; }
    public DateTime CreatedAt { get; set; }
    public Course? Course { get; set; }
    public LearningPath? LearningPath { get; set; }
    public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
    public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}

public class QuizQuestion
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public int Points { get; set; }
    public Quiz Quiz { get; set; } = null!;
}

public class QuizAttempt
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public int UserId { get; set; }
    public int Score { get; set; }
    public bool Passed { get; set; }
    public DateTime AttemptedAt { get; set; }
    public Quiz Quiz { get; set; } = null!;
    public User User { get; set; } = null!;
}
