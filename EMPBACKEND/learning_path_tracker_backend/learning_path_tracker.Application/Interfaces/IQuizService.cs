using learning_path_tracker.Application.DTOs.Quizzes;

namespace learning_path_tracker.Application.Interfaces;

public interface IQuizService
{
    Task<List<QuizDto>> GetAllQuizzesAsync();
    Task<QuizDto?> GetQuizByIdAsync(int id);
    Task<QuizDto> CreateQuizAsync(CreateQuizDto dto);
    Task<QuizDto?> UpdateQuizAsync(int id, UpdateQuizDto dto);
    Task<bool> DeleteQuizAsync(int id);
    Task<QuizAttemptResultDto> SubmitQuizAttemptAsync(int userId, int quizId, SubmitQuizDto dto);
    Task<object> CheckQuizAttemptsAsync(int userId, int quizId);
    Task<QuizDto?> GetLearningPathFinalQuizAsync(int learningPathId, int userId);
}
