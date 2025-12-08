using learning_path_tracker.Application.DTOs.Courses;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Data.Repositories;
using learning_path_tracker.Domain.Entities;
using learning_path_tracker.Database;
using Microsoft.EntityFrameworkCore;
using AllCourseStats = learning_path_tracker.Application.DTOs.Statistics.CourseStatisticsDto;

namespace learning_path_tracker.Application.Services;
//TODO: Add global unhandled exception handling and log it to a file 
public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly AppDbContext _context;

    public CourseService(ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository, AppDbContext context)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _context = context;
    }

    public async Task<List<CourseDto>> GetAllCoursesAsync()
    {
        var courses = await _courseRepository.GetAllAsync();
        var courseDtos = new List<CourseDto>();

        foreach (var c in courses)
        {
            var enrollments = await _enrollmentRepository.GetByCourseIdAsync(c.Id);
            courseDtos.Add(new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Instructor = c.Instructor,
                DurationHours = c.DurationHours,
                Category = c.Category,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                YouTubeVideoId = c.YouTubeVideoId,
                YouTubeUrl = c.YouTubeUrl,
                ThumbnailUrl = c.ThumbnailUrl,
                VideoDuration = c.VideoDuration,
                EnrollmentCount = enrollments.Count
            });
        }

        return courseDtos;
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto)
    {
        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            Instructor = dto.Instructor,
            DurationHours = dto.DurationHours,
            Category = dto.Category,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            YouTubeVideoId = dto.YouTubeVideoId,
            YouTubeUrl = dto.YouTubeUrl,
            ThumbnailUrl = dto.ThumbnailUrl,
            VideoDuration = dto.VideoDuration
        };

        var created = await _courseRepository.AddAsync(course);

        return new CourseDto
        {
            Id = created.Id,
            Title = created.Title,
            Description = created.Description,
            Instructor = created.Instructor,
            DurationHours = created.DurationHours,
            Category = created.Category,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt,
            YouTubeVideoId = created.YouTubeVideoId,
            YouTubeUrl = created.YouTubeUrl,
            ThumbnailUrl = created.ThumbnailUrl,
            VideoDuration = created.VideoDuration
        };
    }

    public async Task<CourseDto?> UpdateCourseAsync(int id, UpdateCourseDto dto)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null) return null;

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Instructor = dto.Instructor;
        course.DurationHours = dto.DurationHours;
        course.Category = dto.Category;
        course.IsActive = dto.IsActive;
        course.YouTubeVideoId = dto.YouTubeVideoId;
        course.YouTubeUrl = dto.YouTubeUrl;
        course.ThumbnailUrl = dto.ThumbnailUrl;
        course.VideoDuration = dto.VideoDuration;

        var updated = await _courseRepository.UpdateAsync(course);

        return new CourseDto
        {
            Id = updated.Id,
            Title = updated.Title,
            Description = updated.Description,
            Instructor = updated.Instructor,
            DurationHours = updated.DurationHours,
            Category = updated.Category,
            IsActive = updated.IsActive,
            CreatedAt = updated.CreatedAt,
            YouTubeVideoId = updated.YouTubeVideoId,
            YouTubeUrl = updated.YouTubeUrl,
            ThumbnailUrl = updated.ThumbnailUrl,
            VideoDuration = updated.VideoDuration
        };
    }

    public async Task<object> CheckCourseDeleteImpactAsync(int id)
    {
        var certificates = await _context.Certificates
            .Include(c => c.User)
            .Where(c => c.CourseId == id)
            .Select(c => new { c.User.FullName, c.CertificateId })
            .ToListAsync();

        return new { hasCertificates = certificates.Any(), certificateCount = certificates.Count, employees = certificates };
    }

    public async Task<bool> DeleteCourseAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null) return false;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var learningPathCourses = await _context.LearningPathCourses.Where(lpc => lpc.CourseId == id).ToListAsync();
            _context.LearningPathCourses.RemoveRange(learningPathCourses);

            var modules = await _context.Modules.Where(m => m.CourseId == id).ToListAsync();
            _context.Modules.RemoveRange(modules);

            var quizzes = await _context.Quizzes.Where(q => q.CourseId == id).ToListAsync();
            foreach (var quiz in quizzes)
            {
                var quizAttempts = await _context.QuizAttempts.Where(qa => qa.QuizId == quiz.Id).ToListAsync();
                _context.QuizAttempts.RemoveRange(quizAttempts);
                
                var quizQuestions = await _context.QuizQuestions.Where(qq => qq.QuizId == quiz.Id).ToListAsync();
                _context.QuizQuestions.RemoveRange(quizQuestions);
            }
            _context.Quizzes.RemoveRange(quizzes);

            var certRequests = await _context.CertificateRequests.Where(cr => cr.CourseId == id).ToListAsync();
            _context.CertificateRequests.RemoveRange(certRequests);

            var certificates = await _context.Certificates.Where(c => c.CourseId == id).ToListAsync();
            _context.Certificates.RemoveRange(certificates);

            var enrollments = await _context.Enrollments.Where(e => e.CourseId == id).ToListAsync();
            _context.Enrollments.RemoveRange(enrollments);

            await _context.SaveChangesAsync();
            await _courseRepository.DeleteAsync(id);
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<CourseStatisticsDto?> GetCourseStatisticsAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null) return null;

        var enrollments = await _enrollmentRepository.GetByCourseIdAsync(id);
        var completed = enrollments.Count(e => e.CompletedAt != null);
        var active = enrollments.Count(e => e.CompletedAt == null);
        var avgProgress = enrollments.Any() ? enrollments.Average(e => e.Progress) : 0;

        return new CourseStatisticsDto
        {
            TotalEnrollments = enrollments.Count,
            CompletedEnrollments = completed,
            ActiveEnrollments = active,
            AverageProgress = avgProgress
        };
    }

    public async Task<AllCourseStats> GetAllCourseStatisticsAsync()
    {
        var courses = await _courseRepository.GetAllAsync();
        var totalEnrollments = 0;
        
        foreach (var course in courses)
        {
            var enrollments = await _enrollmentRepository.GetByCourseIdAsync(course.Id);
            totalEnrollments += enrollments.Count;
        }

        return new AllCourseStats
        {
            TotalCourses = courses.Count,
            ActiveCourses = courses.Count(c => c.IsActive),
            DraftCourses = courses.Count(c => !c.IsActive),
            TotalEnrollments = totalEnrollments
        };
    }

    public async Task<BulkUploadResult> BulkUploadCoursesAsync(List<string> youTubeUrls)
    {
        var result = new BulkUploadResult
        {
            TotalProcessed = youTubeUrls.Count
        };

        var existingCourses = await _courseRepository.GetAllAsync();
        var existingUrls = existingCourses.Select(c => c.YouTubeUrl).ToHashSet();

        foreach (var url in youTubeUrls)
        {
            if (existingUrls.Contains(url))
            {
                result.SkippedCount++;
                result.SkippedUrls.Add(url);
                continue;
            }

            result.SuccessCount++;
        }

        return result;
    }
}
