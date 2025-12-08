using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Data.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly AppDbContext _context;

    public QuizRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Quiz>> GetAllAsync()
    {
        return await _context.Quizzes
            .Include(q => q.Questions)
            .Include(q => q.Course)
            .Include(q => q.LearningPath)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<Quiz?> GetByIdAsync(int id)
    {
        return await _context.Quizzes
            .Include(q => q.Questions)
            .Include(q => q.Course)
            .Include(q => q.LearningPath)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<Quiz?> GetByCourseIdAsync(int courseId)
    {
        return await _context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.CourseId == courseId);
    }

    public async Task<Quiz?> GetByLearningPathIdAsync(int learningPathId, bool isFinalQuiz)
    {
        return await _context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.LearningPathId == learningPathId && q.IsFinalQuiz == isFinalQuiz);
    }

    public async Task<Quiz> AddAsync(Quiz quiz)
    {
        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();
        return quiz;
    }

    public async Task<Quiz> UpdateAsync(Quiz quiz)
    {
        _context.Quizzes.Update(quiz);
        await _context.SaveChangesAsync();
        return quiz;
    }

    public async Task DeleteAsync(int id)
    {
        var quiz = await _context.Quizzes.FindAsync(id);
        if (quiz != null)
        {
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<QuizAttempt> AddAttemptAsync(QuizAttempt attempt)
    {
        _context.QuizAttempts.Add(attempt);
        await _context.SaveChangesAsync();
        return attempt;
    }

    public async Task<QuizAttempt?> GetUserLastAttemptAsync(int quizId, int userId)
    {
        return await _context.QuizAttempts
            .Where(a => a.QuizId == quizId && a.UserId == userId)
            .OrderByDescending(a => a.AttemptedAt)
            .FirstOrDefaultAsync();
    }
}
