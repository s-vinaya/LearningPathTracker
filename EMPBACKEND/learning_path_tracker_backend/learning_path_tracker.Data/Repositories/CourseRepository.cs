using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Data.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Course>> GetAllAsync()
    {
        return await _context.Courses.ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        return await _context.Courses.FindAsync(id);
    }

    public async Task<Course> AddAsync(Course course)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return course;
    }

    public async Task<Course> UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
        return course;
    }

    public async Task DeleteAsync(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            var quizzes = await _context.Quizzes.Where(q => q.CourseId == id).ToListAsync();
            foreach (var quiz in quizzes)
            {
                var attempts = await _context.QuizAttempts.Where(qa => qa.QuizId == quiz.Id).ToListAsync();
                _context.QuizAttempts.RemoveRange(attempts);
                
                var questions = await _context.QuizQuestions.Where(qq => qq.QuizId == quiz.Id).ToListAsync();
                _context.QuizQuestions.RemoveRange(questions);
            }
            _context.Quizzes.RemoveRange(quizzes);
            
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetActiveCourseCountAsync()
    {
        return await _context.Courses.CountAsync(c => c.IsActive);
    }

    public async Task<int> GetCompletedCourseCountAsync()
    {
        return await _context.Enrollments
            .Include(e => e.Course)
            .CountAsync(e => e.Progress >= 80 && (!e.Course.QuizId.HasValue || e.QuizPassed));
    }
}
