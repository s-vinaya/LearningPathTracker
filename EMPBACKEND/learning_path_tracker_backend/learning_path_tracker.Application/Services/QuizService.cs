using learning_path_tracker.Application.DTOs.Quizzes;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Data.Repositories;
using learning_path_tracker.Domain.Entities;
using learning_path_tracker.Database;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Application.Services;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly AppDbContext _context;
    private readonly PointsCalculationService _pointsService;

    public QuizService(IQuizRepository quizRepository, IEnrollmentRepository enrollmentRepository, AppDbContext context, PointsCalculationService pointsService)
    {
        _quizRepository = quizRepository;
        _enrollmentRepository = enrollmentRepository;
        _context = context;
        _pointsService = pointsService;
    }

    public async Task<List<QuizDto>> GetAllQuizzesAsync()
    {
        var quizzes = await _quizRepository.GetAllAsync();
        return quizzes.Select(q => new QuizDto
        {
            Id = q.Id,
            Title = q.Title,
            Description = q.Description,
            CourseId = q.CourseId,
            CourseName = q.Course?.Title,
            LearningPathId = q.LearningPathId,
            LearningPathName = q.LearningPath?.Title,
            PassingScore = q.PassingScore,
            TimeLimit = q.TimeLimit,
            IsActive = q.IsActive,
            IsFinalQuiz = q.IsFinalQuiz,
            CreatedAt = q.CreatedAt,
            Questions = q.Questions.Select(qn => new QuizQuestionDto
            {
                Id = qn.Id,
                QuizId = qn.QuizId,
                Question = qn.Question,
                OptionA = qn.OptionA,
                OptionB = qn.OptionB,
                OptionC = qn.OptionC,
                OptionD = qn.OptionD,
                CorrectAnswer = qn.CorrectAnswer,
                Points = qn.Points
            }).ToList()
        }).ToList();
    }

    public async Task<QuizDto?> GetQuizByIdAsync(int id)
    {
        var quiz = await _quizRepository.GetByIdAsync(id);
        if (quiz == null) return null;

        return new QuizDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            CourseId = quiz.CourseId,
            CourseName = quiz.Course?.Title,
            LearningPathId = quiz.LearningPathId,
            LearningPathName = quiz.LearningPath?.Title,
            PassingScore = quiz.PassingScore,
            TimeLimit = quiz.TimeLimit,
            IsActive = quiz.IsActive,
            IsFinalQuiz = quiz.IsFinalQuiz,
            CreatedAt = quiz.CreatedAt,
            Questions = quiz.Questions.Select(q => new QuizQuestionDto
            {
                Id = q.Id,
                QuizId = q.QuizId,
                Question = q.Question,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                CorrectAnswer = q.CorrectAnswer,
                Points = q.Points
            }).ToList()
        };
    }

    public async Task<QuizDto> CreateQuizAsync(CreateQuizDto dto)
    {
        var quiz = new Quiz
        {
            Title = dto.Title,
            Description = dto.Description,
            CourseId = dto.CourseId,
            LearningPathId = dto.LearningPathId,
            PassingScore = dto.PassingScore,
            TimeLimit = dto.TimeLimit,
            IsActive = dto.IsActive,
            IsFinalQuiz = dto.IsFinalQuiz,
            CreatedAt = DateTime.UtcNow,
            Questions = dto.Questions.Select(q => new QuizQuestion
            {
                Question = q.Question,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                CorrectAnswer = q.CorrectAnswer,
                Points = q.Points
            }).ToList()
        };

        var created = await _quizRepository.AddAsync(quiz);

        if (created.CourseId.HasValue)
        {
            var course = await _context.Courses.FindAsync(created.CourseId.Value);
            if (course != null)
            {
                course.QuizId = created.Id;
                await _context.SaveChangesAsync();
            }
        }

        var reloaded = await _quizRepository.GetByIdAsync(created.Id);

        return new QuizDto
        {
            Id = reloaded.Id,
            Title = reloaded.Title,
            Description = reloaded.Description,
            CourseId = reloaded.CourseId,
            CourseName = reloaded.Course?.Title,
            LearningPathId = reloaded.LearningPathId,
            LearningPathName = reloaded.LearningPath?.Title,
            PassingScore = reloaded.PassingScore,
            TimeLimit = reloaded.TimeLimit,
            IsActive = reloaded.IsActive,
            IsFinalQuiz = reloaded.IsFinalQuiz,
            CreatedAt = reloaded.CreatedAt,
            Questions = reloaded.Questions.Select(q => new QuizQuestionDto
            {
                Id = q.Id,
                QuizId = q.QuizId,
                Question = q.Question,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                CorrectAnswer = q.CorrectAnswer,
                Points = q.Points
            }).ToList()
        };
    }

    public async Task<QuizDto?> UpdateQuizAsync(int id, UpdateQuizDto dto)
    {
        var quiz = await _quizRepository.GetByIdAsync(id);
        if (quiz == null) return null;

        quiz.Title = dto.Title;
        quiz.Description = dto.Description;
        quiz.PassingScore = dto.PassingScore;
        quiz.TimeLimit = dto.TimeLimit;
        quiz.IsActive = dto.IsActive;

        var updated = await _quizRepository.UpdateAsync(quiz);

        return new QuizDto
        {
            Id = updated.Id,
            Title = updated.Title,
            Description = updated.Description,
            CourseId = updated.CourseId,
            LearningPathId = updated.LearningPathId,
            PassingScore = updated.PassingScore,
            TimeLimit = updated.TimeLimit,
            IsActive = updated.IsActive,
            IsFinalQuiz = updated.IsFinalQuiz,
            CreatedAt = updated.CreatedAt,
            Questions = updated.Questions.Select(q => new QuizQuestionDto
            {
                Id = q.Id,
                QuizId = q.QuizId,
                Question = q.Question,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                CorrectAnswer = q.CorrectAnswer,
                Points = q.Points
            }).ToList()
        };
    }

    public async Task<bool> DeleteQuizAsync(int id)
    {
        var quiz = await _quizRepository.GetByIdAsync(id);
        if (quiz == null) return false;

        await _quizRepository.DeleteAsync(id);
        return true;
    }

    public async Task<object> CheckQuizAttemptsAsync(int userId, int quizId)
    {
        var istNow = DateTime.UtcNow.AddHours(5).AddMinutes(30);
        var today = istNow.Date;
        var todayAttempts = await _context.QuizAttempts
            .Where(a => a.UserId == userId && a.QuizId == quizId && a.AttemptedAt.AddHours(5).AddMinutes(30).Date == today)
            .CountAsync();

        var canAttempt = todayAttempts < 3;
        var remainingAttempts = Math.Max(0, 3 - todayAttempts);

        if (!canAttempt)
        {
            var midnightIST = today.AddDays(1);
            var midnightUTC = midnightIST.AddHours(-5).AddMinutes(-30);
            var timeUntilReset = midnightIST - istNow;

            return new
            {
                canAttempt = false,
                attemptsUsed = todayAttempts,
                remainingAttempts = 0,
                resetTime = midnightUTC,
                hoursUntilReset = (int)timeUntilReset.TotalHours,
                minutesUntilReset = (int)timeUntilReset.TotalMinutes % 60
            };
        }

        return new
        {
            canAttempt = true,
            attemptsUsed = todayAttempts,
            remainingAttempts = remainingAttempts
        };
    }

    public async Task<QuizAttemptResultDto> SubmitQuizAttemptAsync(int userId, int quizId, SubmitQuizDto dto)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null)
            throw new Exception("Quiz not found");

        var istNow = DateTime.UtcNow.AddHours(5).AddMinutes(30);
        var today = istNow.Date;
        var todayAttempts = await _context.QuizAttempts
            .Where(a => a.UserId == userId && a.QuizId == quizId && a.AttemptedAt.AddHours(5).AddMinutes(30).Date == today)
            .CountAsync();
        if (todayAttempts >= 3)
            throw new Exception("Daily attempt limit reached (3 attempts per day)");

        var totalPoints = quiz.Questions.Sum(q => q.Points);
        var earnedPoints = 0;

        foreach (var question in quiz.Questions)
        {
            if (dto.Answers.TryGetValue(question.Id, out var answer) && answer == question.CorrectAnswer)
            {
                earnedPoints += question.Points;
            }
        }

        var score = totalPoints > 0 ? (earnedPoints * 100) / totalPoints : 0;
        var passed = score >= quiz.PassingScore;

        var attempt = new QuizAttempt
        {
            QuizId = quizId,
            UserId = userId,
            Score = score,
            Passed = passed,
            AttemptedAt = DateTime.UtcNow
        };

        await _quizRepository.AddAttemptAsync(attempt);

        if (passed && quiz.CourseId.HasValue)
        {
            var enrollments = await _enrollmentRepository.GetByUserIdAsync(userId);
            var enrollment = enrollments.FirstOrDefault(e => e.CourseId == quiz.CourseId.Value);
            if (enrollment != null)
            {
                enrollment.QuizPassed = true;
                if (enrollment.Progress >= 80)
                {
                    enrollment.CompletedAt = DateTime.UtcNow;
                    await _pointsService.AwardPointsForCourseCompletion(userId, quiz.CourseId.Value);
                }
                await _enrollmentRepository.UpdateAsync(enrollment);
            }
        }

        if (passed && quiz.IsFinalQuiz && quiz.LearningPathId.HasValue)
        {
            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.EmployeeId == userId && a.LearningPathId == quiz.LearningPathId.Value);
            
            if (assignment != null)
            {
                var learningPath = await _context.LearningPaths
                    .Include(lp => lp.LearningPathCourses)
                    .FirstOrDefaultAsync(lp => lp.Id == quiz.LearningPathId.Value);

                if (learningPath != null)
                {
                    var allCourseIds = learningPath.LearningPathCourses.Select(lpc => lpc.CourseId).ToList();
                    var userEnrollments = await _context.Enrollments
                        .Where(e => e.UserId == userId && allCourseIds.Contains(e.CourseId))
                        .ToListAsync();

                    var allCoursesCompleted = allCourseIds.Count > 0 && 
                        allCourseIds.All(courseId => userEnrollments.Any(e => e.CourseId == courseId && e.CompletedAt != null && e.QuizPassed));

                    if (allCoursesCompleted)
                    {
                        assignment.Status = "Completed";
                        assignment.CompletedDate = DateTime.UtcNow;
                        assignment.ProgressPercent = 100;
                        await _context.SaveChangesAsync();

                        var existingCertificate = await _context.Certificates
                            .FirstOrDefaultAsync(c => c.UserId == userId && c.LearningPathId == quiz.LearningPathId.Value);

                        if (existingCertificate == null)
                        {
                            var user = await _context.Users.FindAsync(userId);
                            var certificate = new Certificate
                            {
                                UserId = userId,
                                EmployeeName = user?.FullName ?? "Unknown",
                                LearningPathId = quiz.LearningPathId.Value,
                                LearningPathName = learningPath.Title,
                                CertificateId = Guid.NewGuid().ToString(),
                                IssuedAt = DateTime.UtcNow,
                                CertificateType = "LearningPath",
                                AverageScore = score
                            };
                            _context.Certificates.Add(certificate);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        return new QuizAttemptResultDto
        {
            Score = score,
            Passed = passed,
            TotalPoints = totalPoints,
            EarnedPoints = earnedPoints,
            AttemptedAt = attempt.AttemptedAt,
            AttemptsToday = todayAttempts + 1,
            RemainingAttempts = 2 - todayAttempts
        };
    }

    public async Task<QuizDto?> GetLearningPathFinalQuizAsync(int learningPathId, int userId)
    {
        var learningPath = await _context.LearningPaths
            .Include(lp => lp.LearningPathCourses)
            .FirstOrDefaultAsync(lp => lp.Id == learningPathId);

        if (learningPath == null) return null;

        var allCourseIds = learningPath.LearningPathCourses.Select(lpc => lpc.CourseId).ToList();
        var userEnrollments = await _context.Enrollments
            .Where(e => e.UserId == userId && allCourseIds.Contains(e.CourseId))
            .ToListAsync();

        var allCoursesCompleted = allCourseIds.Count > 0 && 
            allCourseIds.All(courseId => userEnrollments.Any(e => e.CourseId == courseId && e.CompletedAt != null && e.QuizPassed));

        if (!allCoursesCompleted) return null;

        var finalQuiz = await _context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.LearningPathId == learningPathId && q.IsFinalQuiz && q.IsActive);

        if (finalQuiz == null) return null;

        return new QuizDto
        {
            Id = finalQuiz.Id,
            Title = finalQuiz.Title,
            Description = finalQuiz.Description,
            LearningPathId = finalQuiz.LearningPathId,
            LearningPathName = learningPath.Title,
            PassingScore = finalQuiz.PassingScore,
            TimeLimit = finalQuiz.TimeLimit,
            IsActive = finalQuiz.IsActive,
            IsFinalQuiz = finalQuiz.IsFinalQuiz,
            CreatedAt = finalQuiz.CreatedAt,
            Questions = finalQuiz.Questions.Select(q => new QuizQuestionDto
            {
                Id = q.Id,
                QuizId = q.QuizId,
                Question = q.Question,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                CorrectAnswer = q.CorrectAnswer,
                Points = q.Points
            }).ToList()
        };
    }
}
