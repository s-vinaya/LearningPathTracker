namespace learning_path_tracker.Application.DTOs.Quizzes;

public class QuizDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? CourseId { get; set; }
    public string? CourseName { get; set; }
    public int? LearningPathId { get; set; }
    public string? LearningPathName { get; set; }
    public int PassingScore { get; set; }
    public int TimeLimit { get; set; }
    public bool IsActive { get; set; }
    public bool IsFinalQuiz { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<QuizQuestionDto> Questions { get; set; } = new();
}

public class QuizQuestionDto
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public string? CorrectAnswer { get; set; }
    public int Points { get; set; }
}

public class CreateQuizDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? CourseId { get; set; }
    public int? LearningPathId { get; set; }
    public int PassingScore { get; set; }
    public int TimeLimit { get; set; }
    public bool IsActive { get; set; }
    public bool IsFinalQuiz { get; set; }
    public List<CreateQuizQuestionDto> Questions { get; set; } = new();
}

public class CreateQuizQuestionDto
{
    public string Question { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public int Points { get; set; }
}

public class UpdateQuizDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PassingScore { get; set; }
    public int TimeLimit { get; set; }
    public bool IsActive { get; set; }
}

public class SubmitQuizDto
{
    public Dictionary<int, string> Answers { get; set; } = new();
}

public class QuizAttemptResultDto
{
    public int Score { get; set; }
    public bool Passed { get; set; }
    public int TotalPoints { get; set; }
    public int EarnedPoints { get; set; }
    public DateTime AttemptedAt { get; set; }
    public int AttemptsToday { get; set; }
    public int RemainingAttempts { get; set; }
}
