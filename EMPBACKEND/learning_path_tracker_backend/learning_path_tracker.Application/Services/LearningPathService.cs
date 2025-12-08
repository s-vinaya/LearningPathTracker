using learning_path_tracker.Application.DTOs.LearningPaths;
using learning_path_tracker.Application.DTOs.Statistics;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Data.Repositories;
using learning_path_tracker.Domain.Entities;
using learning_path_tracker.Database;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Application.Services;

public class LearningPathService : ILearningPathService
{
    private readonly ILearningPathRepository _learningPathRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly AppDbContext _context;
    private readonly ICourseRepository _courseRepository;

    public LearningPathService(ILearningPathRepository learningPathRepository, IModuleRepository moduleRepository, IEnrollmentRepository enrollmentRepository, AppDbContext context, ICourseRepository courseRepository)
    {
        _learningPathRepository = learningPathRepository;
        _moduleRepository = moduleRepository;
        _enrollmentRepository = enrollmentRepository;
        _context = context;
        _courseRepository = courseRepository;
    }

    public async Task<List<LearningPathDto>> GetAllLearningPathsAsync()
    {
        var learningPaths = await _learningPathRepository.GetAllAsync();
        var result = new List<LearningPathDto>();
        
        foreach (var lp in learningPaths)
        {
            var enrollmentCount = await _context.Assignments.CountAsync(a => a.LearningPathId == lp.Id);
            
            result.Add(new LearningPathDto
            {
                Id = lp.Id,
                Title = lp.Title,
                Description = lp.Description,
                Level = lp.Level,
                IsActive = lp.IsActive,
                CreatedAt = lp.CreatedAt,
                EnrollmentCount = enrollmentCount,
                Modules = lp.LearningPathCourses?.Select(lpc => new ModuleDto
                {
                    Id = lpc.Course.Id,
                    Title = lpc.Course.Title,
                    Description = lpc.Course.Description,
                    Order = lpc.Order,
                    LearningPathId = lp.Id
                }).OrderBy(m => m.Order).ToList() ?? new List<ModuleDto>()
            });
        }
        
        return result;
    }

    public async Task<LearningPathDto> CreateLearningPathAsync(CreateLearningPathDto dto)
    {
        var learningPath = new LearningPath
        {
            Title = dto.Title,
            Description = dto.Description,
            Level = dto.Level,
            IsActive = dto.IsActive,
            CreatedOn = DateTime.UtcNow
        };

        var created = await _learningPathRepository.AddAsync(learningPath);

        var modules = new List<ModuleDto>();
        foreach (var moduleDto in dto.Modules)
        {
            // Try to find matching course by title
            var matchingCourse = await _context.Courses
                .FirstOrDefaultAsync(c => c.Title == moduleDto.Title);
            
            var module = new Module
            {
                Title = moduleDto.Title,
                Description = moduleDto.Description,
                Order = moduleDto.Order,
                LearningPathId = created.Id,
                CourseId = matchingCourse?.Id
            };
            var createdModule = await _moduleRepository.AddAsync(module);
            modules.Add(new ModuleDto
            {
                Id = createdModule.Id,
                Title = createdModule.Title,
                Description = createdModule.Description,
                Order = createdModule.Order,
                LearningPathId = createdModule.LearningPathId
            });
        }

        return new LearningPathDto
        {
            Id = created.Id,
            Title = created.Title,
            Description = created.Description,
            Level = created.Level,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt,
            EnrollmentCount = 0,
            Modules = modules
        };
    }

    public async Task<LearningPathDto?> UpdateLearningPathAsync(int id, UpdateLearningPathDto dto)
    {
        var learningPath = await _learningPathRepository.GetByIdAsync(id);
        if (learningPath == null) return null;

        learningPath.Title = dto.Title;
        learningPath.Description = dto.Description;
        learningPath.Level = dto.Level;
        learningPath.IsActive = dto.IsActive;

        var updated = await _learningPathRepository.UpdateAsync(learningPath);

        return new LearningPathDto
        {
            Id = updated.Id,
            Title = updated.Title,
            Description = updated.Description,
            Level = updated.Level,
            IsActive = updated.IsActive,
            CreatedAt = updated.CreatedAt,
            EnrollmentCount = 0,
            Modules = updated.Modules.Select(m => new ModuleDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Order = m.Order,
                LearningPathId = m.LearningPathId
            }).ToList()
        };
    }

    public async Task<bool> DeleteLearningPathAsync(int id)
    {
        var learningPath = await _learningPathRepository.GetByIdAsync(id);
        if (learningPath == null) return false;

        await _learningPathRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> UpdateModulesAsync(int learningPathId, List<CreateModuleDto> modules)
    {
        var learningPath = await _learningPathRepository.GetByIdAsync(learningPathId);
        if (learningPath == null) return false;

        var existingModules = await _moduleRepository.GetByLearningPathIdAsync(learningPathId);
        foreach (var module in existingModules)
        {
            await _moduleRepository.DeleteAsync(module.Id);
        }

        foreach (var dto in modules)
        {
            // Try to find matching course by title
            var matchingCourse = await _context.Courses
                .FirstOrDefaultAsync(c => c.Title == dto.Title);
            
            var module = new Module
            {
                Title = dto.Title,
                Description = dto.Description,
                Order = dto.Order,
                LearningPathId = learningPathId,
                CourseId = matchingCourse?.Id
            };
            await _moduleRepository.AddAsync(module);
        }

        return true;
    }

    public async Task<ModuleDto> AddModuleAsync(int learningPathId, CreateModuleDto dto)
    {
        var module = new Module
        {
            Title = dto.Title,
            Description = dto.Description,
            Order = dto.Order,
            LearningPathId = learningPathId
        };

        var created = await _moduleRepository.AddAsync(module);

        return new ModuleDto
        {
            Id = created.Id,
            Title = created.Title,
            Description = created.Description,
            Order = created.Order,
            LearningPathId = created.LearningPathId
        };
    }

    public async Task<ModuleDto?> UpdateModuleAsync(int moduleId, UpdateModuleDto dto)
    {
        var module = await _moduleRepository.GetByIdAsync(moduleId);
        if (module == null) return null;

        module.Title = dto.Title;
        module.Description = dto.Description;
        module.Order = dto.Order;

        var updated = await _moduleRepository.UpdateAsync(module);

        return new ModuleDto
        {
            Id = updated.Id,
            Title = updated.Title,
            Description = updated.Description,
            Order = updated.Order,
            LearningPathId = updated.LearningPathId
        };
    }

    public async Task<bool> DeleteModuleAsync(int moduleId)
    {
        var module = await _moduleRepository.GetByIdAsync(moduleId);
        if (module == null) return false;

        await _moduleRepository.DeleteAsync(moduleId);
        return true;
    }

    public async Task<LearningPathStatisticsDto> GetStatisticsAsync()
    {
        var learningPaths = await _learningPathRepository.GetAllAsync();
        var totalEnrollments = await _enrollmentRepository.GetTotalEnrollmentsAsync();
        var totalCompletions = await _enrollmentRepository.GetTotalCompletionsAsync();
        
        return new LearningPathStatisticsDto
        {
            TotalPaths = learningPaths.Count,
            ActivePaths = learningPaths.Count(lp => lp.IsActive),
            TotalEnrollments = totalEnrollments,
            TotalCompletions = totalCompletions
        };
    }

    public async Task<List<object>> GetLearningPathsWithCoursesAsync()
    {
        var learningPaths = await _context.LearningPaths
            .Include(lp => lp.LearningPathCourses)
                .ThenInclude(lpc => lpc.Course)
            .Include(lp => lp.Modules)
            .ToListAsync();
            
        var result = new List<object>();
        
        foreach (var lp in learningPaths)
        {
            List<object> courses;
            
            if (lp.LearningPathCourses?.Any() == true)
            {
                courses = lp.LearningPathCourses.OrderBy(lpc => lpc.Order).Select(lpc => new
                {
                    Id = lpc.Course.Id,
                    Title = lpc.Course.Title,
                    Description = lpc.Course.Description,
                    Instructor = lpc.Course.Instructor,
                    DurationHours = lpc.Course.DurationHours,
                    Category = lpc.Course.Category,
                    ThumbnailUrl = lpc.Course.ThumbnailUrl,
                    YouTubeUrl = lpc.Course.YouTubeUrl,
                    QuizId = lpc.Course.QuizId,
                    HasQuiz = lpc.Course.QuizId.HasValue,
                    Order = lpc.Order,
                    LearningPathId = lp.Id
                } as object).ToList();
            }
            else
            {
                var moduleIds = lp.Modules.Select(m => m.Id).ToList();
                var modulesWithCourses = await _context.Modules
                    .Where(m => moduleIds.Contains(m.Id))
                    .Include(m => m.Course)
                    .OrderBy(m => m.Order)
                    .ToListAsync();
                    
                courses = modulesWithCourses.Select(m => new
                {
                    Id = m.Id,
                    CourseId = m.CourseId,
                    Title = m.Title,
                    Description = m.Description,
                    Instructor = m.Course?.Instructor,
                    DurationHours = m.Course?.DurationHours ?? 0,
                    Category = m.Course?.Category,
                    ThumbnailUrl = m.Course?.ThumbnailUrl,
                    YouTubeUrl = m.Course?.YouTubeUrl,
                    Order = m.Order,
                    LearningPathId = m.LearningPathId
                } as object).ToList();
            }
                
            var enrollmentCount = await _context.Assignments.CountAsync(a => a.LearningPathId == lp.Id);
            
            result.Add(new
            {
                lp.Id,
                lp.Title,
                lp.Description,
                lp.Level,
                lp.IsActive,
                CreatedAt = lp.CreatedOn,
                EstimatedHours = lp.EstimatedHours,
                CreatedBy = lp.CreatedBy,
                EnrollmentCount = enrollmentCount,
                Courses = courses
            });
        }
        
        return result;
    }

    public async Task<List<object>> GetEmployeeLearningPathsAsync(int employeeId)
    {
        var enrolledCourseIds = await _context.Enrollments
            .Where(e => e.UserId == employeeId)
            .Select(e => e.CourseId)
            .ToListAsync();

        var learningPaths = await _context.LearningPaths
            .Include(lp => lp.LearningPathCourses)
            .ThenInclude(lpc => lpc.Course)
            .Where(lp => lp.IsActive && lp.LearningPathCourses.Any(lpc => enrolledCourseIds.Contains(lpc.CourseId)))
            .ToListAsync();
            
        return learningPaths.Select(lp => new
        {
            lp.Id,
            lp.Title,
            lp.Description,
            lp.Level,
            lp.IsActive,
            CreatedAt = lp.CreatedOn,
            EstimatedHours = lp.EstimatedHours,
            Courses = lp.LearningPathCourses.OrderBy(lpc => lpc.Order).Select(lpc => new
            {
                Id = lpc.Course.Id,
                Title = lpc.Course.Title,
                Description = lpc.Course.Description,
                Order = lpc.Order,
                LearningPathId = lp.Id
            }).ToList()
        }).ToList<object>();
    }

    public async Task<List<object>> GetManagerLearningPathsAsync(int managerId)
    {
        var teamMemberIds = await _context.Users
            .Where(u => u.ManagerId == managerId)
            .Select(u => u.Id)
            .ToListAsync();

        var enrolledCourseIds = await _context.Enrollments
            .Where(e => teamMemberIds.Contains(e.UserId))
            .Select(e => e.CourseId)
            .ToListAsync();

        var learningPaths = await _context.LearningPaths
            .Include(lp => lp.LearningPathCourses)
            .ThenInclude(lpc => lpc.Course)
            .Where(lp => lp.IsActive && lp.LearningPathCourses.Any(lpc => enrolledCourseIds.Contains(lpc.CourseId)))
            .ToListAsync();
            
        return learningPaths.Select(lp => new
        {
            lp.Id,
            lp.Title,
            lp.Description,
            lp.Level,
            lp.IsActive,
            CreatedAt = lp.CreatedOn,
            EstimatedHours = lp.EstimatedHours,
            Courses = lp.LearningPathCourses.OrderBy(lpc => lpc.Order).Select(lpc => new
            {
                Id = lpc.Course.Id,
                Title = lpc.Course.Title,
                Description = lpc.Course.Description,
                Order = lpc.Order,
                LearningPathId = lp.Id
            }).ToList()
        }).ToList<object>();
    }

    public async Task<object> CreateLearningPathWithCoursesAsync(CreateLearningPathWithCoursesDto dto)
    {
        var learningPath = new LearningPath
        {
            Title = dto.Title,
            Description = dto.Description,
            EstimatedHours = dto.EstimatedHours,
            CreatedBy = dto.CreatedBy,
            CreatedOn = DateTime.UtcNow,
            IsActive = true
        };

        _context.LearningPaths.Add(learningPath);
        await _context.SaveChangesAsync();

        var order = 1;
        foreach (var courseId in dto.CourseIds)
        {
            var learningPathCourse = new LearningPathCourse
            {
                LearningPathId = learningPath.Id,
                CourseId = courseId,
                Order = order++
            };
            _context.LearningPathCourses.Add(learningPathCourse);
        }

        await _context.SaveChangesAsync();

        return new
        {
            learningPath.Id,
            learningPath.Title,
            learningPath.Description,
            learningPath.EstimatedHours,
            learningPath.CreatedBy,
            learningPath.CreatedOn
        };
    }

    public async Task<bool> AddCoursesToPathAsync(AddCoursesToPathDto dto)
    {
        var learningPath = await _context.LearningPaths.FindAsync(dto.LearningPathId);
        if (learningPath == null) return false;

        var maxOrder = await _context.LearningPathCourses
            .Where(lpc => lpc.LearningPathId == dto.LearningPathId)
            .MaxAsync(lpc => (int?)lpc.Order) ?? 0;

        var order = maxOrder + 1;
        foreach (var courseId in dto.CourseIds)
        {
            var exists = await _context.LearningPathCourses
                .AnyAsync(lpc => lpc.LearningPathId == dto.LearningPathId && lpc.CourseId == courseId);
            
            if (!exists)
            {
                var learningPathCourse = new LearningPathCourse
                {
                    LearningPathId = dto.LearningPathId,
                    CourseId = courseId,
                    Order = order++
                };
                _context.LearningPathCourses.Add(learningPathCourse);
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateCourseOrderAsync(UpdateCourseOrderDto dto)
    {
        var learningPath = await _context.LearningPaths.FindAsync(dto.LearningPathId);
        if (learningPath == null) return false;

        foreach (var courseOrder in dto.CourseOrders)
        {
            var learningPathCourse = await _context.LearningPathCourses
                .FirstOrDefaultAsync(lpc => lpc.LearningPathId == dto.LearningPathId && lpc.CourseId == courseOrder.CourseId);
            
            if (learningPathCourse != null)
            {
                learningPathCourse.Order = courseOrder.Order;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveCourseFromPathAsync(int learningPathId, int courseId)
    {
        var learningPathCourse = await _context.LearningPathCourses
            .FirstOrDefaultAsync(lpc => lpc.LearningPathId == learningPathId && lpc.CourseId == courseId);
        
        if (learningPathCourse == null) return false;

        _context.LearningPathCourses.Remove(learningPathCourse);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<object>> GetAvailableCoursesAsync()
    {
        var courses = await _context.Courses
            .Where(c => c.IsActive)
            .OrderBy(c => c.Title)
            .ToListAsync();

        return courses.Select(c => new
        {
            c.Id,
            c.Title,
            c.Description,
            c.DurationHours,
            c.Category,
            c.Instructor
        }).ToList<object>();
    }
}