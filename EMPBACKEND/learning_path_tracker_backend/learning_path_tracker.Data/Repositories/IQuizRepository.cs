using learning_path_tracker.Domain.Entities;

namespace learning_path_tracker.Data.Repositories;

public interface IQuizRepository
{
    Task<List<Quiz>> GetAllAsync();
    Task<Quiz?> GetByIdAsync(int id);
    Task<Quiz?> GetByCourseIdAsync(int courseId);
    Task<Quiz?> GetByLearningPathIdAsync(int learningPathId, bool isFinalQuiz);
    Task<Quiz> AddAsync(Quiz quiz);
    Task<Quiz> UpdateAsync(Quiz quiz);
    Task DeleteAsync(int id);
    Task<QuizAttempt> AddAttemptAsync(QuizAttempt attempt);
    Task<QuizAttempt?> GetUserLastAttemptAsync(int quizId, int userId);
}
