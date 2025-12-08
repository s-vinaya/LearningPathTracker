using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Application.DTOs.Common;
using learning_path_tracker.Application.Logging;
using learning_path_tracker.Api.Attributes;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/employee")]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly FileLogger _logger;

    public EmployeeController(IEmployeeService employeeService, FileLogger logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    [HttpGet("{id}/learning-paths")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> GetEmployeeLearningPaths(int id)
    {
        try
        {
            _logger.LogInfo("Controller", $"GetEmployeeLearningPaths: userId={id}");
            var result = await _employeeService.GetEmployeeLearningPathsAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"GetEmployeeLearningPaths failed for userId={id}", ex);
            return StatusCode(500, new { message = "Error retrieving learning paths" });
        }
    }

    [HttpGet("learning-paths/{pathId}/modules")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> GetLearningPathCourses(int pathId)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value 
                ?? User.FindFirst("id")?.Value 
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogInfo("Controller", $"GetLearningPathCourses: pathId={pathId} (no user context)");
                var courses = await _employeeService.GetLearningPathCoursesAsync(pathId);
                return Ok(courses);
            }

            _logger.LogInfo("Controller", $"GetLearningPathCourses: userId={userId}, pathId={pathId}");
            var coursesWithLocking = await _employeeService.GetLearningPathCoursesAsync(userId, pathId);
            return Ok(coursesWithLocking);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"GetLearningPathCourses failed for pathId={pathId}", ex);
            return StatusCode(500, new { message = "Error retrieving courses" });
        }
    }

    [HttpGet("courses/available")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> GetAvailableCourses()
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value 
                ?? User.FindFirst("id")?.Value 
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            int? userId = null;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId))
            {
                userId = parsedUserId;
            }

            _logger.LogInfo("Controller", $"GetAvailableCourses: userId={userId}");
            var courses = await _employeeService.GetAvailableCoursesAsync(userId);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetAvailableCourses failed", ex);
            return StatusCode(500, new { message = "Error retrieving available courses" });
        }
    }

    [HttpGet("courses/enrolled")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> GetEnrolledCourses()
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value 
                ?? User.FindFirst("id")?.Value 
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Controller", "GetEnrolledCourses: User not authenticated");
                return Unauthorized(new { message = "User not authenticated" });
            }

            _logger.LogInfo("Controller", $"GetEnrolledCourses: userId={userId}");
            var courses = await _employeeService.GetEnrolledCoursesAsync(userId);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetEnrolledCourses failed", ex);
            return StatusCode(500, new { message = "Error retrieving enrolled courses" });
        }
    }

    [HttpGet("learning-paths/available")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> GetAvailableLearningPaths()
    {
        try
        {
            _logger.LogInfo("Controller", "GetAvailableLearningPaths called");
            var paths = await _employeeService.GetAvailableLearningPathsAsync();
            return Ok(paths);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetAvailableLearningPaths failed", ex);
            return StatusCode(500, new { message = "Error retrieving learning paths" });
        }
    }

    [HttpPost("courses/{courseId}/enroll")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> EnrollInCourse(int courseId)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value 
                ?? User.FindFirst("id")?.Value 
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Controller", "EnrollInCourse: User not authenticated");
                return Unauthorized(new { message = "User not authenticated" });
            }
            
            _logger.LogInfo("Controller", $"EnrollInCourse: userId={userId}, courseId={courseId}");
            var success = await _employeeService.EnrollInCourseAsync(userId, courseId);
            if (!success)
            {
                _logger.LogWarning("Controller", $"EnrollInCourse: Already enrolled - userId={userId}, courseId={courseId}");
                return BadRequest(new { message = "Already enrolled in this course" });
            }

            _logger.LogInfo("Controller", $"EnrollInCourse successful: userId={userId}, courseId={courseId}");
            return Ok(new { message = "Enrolled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"EnrollInCourse failed: courseId={courseId}", ex);
            return StatusCode(500, new { message = "Error enrolling in course" });
        }
    }

    [HttpPost("learning-paths/{learningPathId}/enroll")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> EnrollInLearningPath(int learningPathId)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value 
                ?? User.FindFirst("id")?.Value 
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Controller", "EnrollInLearningPath: User not authenticated");
                return Unauthorized(new { message = "User not authenticated" });
            }
            
            _logger.LogInfo("Controller", $"EnrollInLearningPath: userId={userId}, pathId={learningPathId}");
            var success = await _employeeService.EnrollInLearningPathAsync(userId, learningPathId);
            if (!success)
            {
                _logger.LogWarning("Controller", $"EnrollInLearningPath: Already enrolled - userId={userId}, pathId={learningPathId}");
                return BadRequest(new { message = "Already enrolled in this learning path" });
            }

            _logger.LogInfo("Controller", $"EnrollInLearningPath successful: userId={userId}, pathId={learningPathId}");
            return Ok(new { message = "Enrolled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"EnrollInLearningPath failed: pathId={learningPathId}", ex);
            return StatusCode(500, new { message = "Error enrolling in learning path" });
        }
    }

    [HttpGet("{userId}/courses/{courseId}")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> GetCourseDetails(int userId, int courseId)
    {
        try
        {
            _logger.LogInfo("Controller", $"GetCourseDetails: userId={userId}, courseId={courseId}");
            var course = await _employeeService.GetCourseDetailsAsync(userId, courseId);
            if (course == null)
            {
                _logger.LogWarning("Controller", $"GetCourseDetails: Course not found - userId={userId}, courseId={courseId}");
                return NotFound(new { message = "Course not found" });
            }
            return Ok(course);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"GetCourseDetails failed: userId={userId}, courseId={courseId}", ex);
            return StatusCode(500, new { message = "Error retrieving course details" });
        }
    }

    [HttpPost("courses/{courseId}/progress")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> UpdateCourseProgress(int courseId, [FromBody] ProgressUpdateDto dto)
    {
        try
        {
            if (dto.Progress < 0 || dto.Progress > 100)
            {
                _logger.LogWarning("Controller", $"UpdateCourseProgress: Invalid progress value={dto.Progress}");
                return BadRequest(new { message = "Progress must be between 0 and 100" });
            }

            var userIdClaim = User.FindFirst("userId")?.Value 
                ?? User.FindFirst("id")?.Value 
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Controller", "UpdateCourseProgress: User not authenticated");
                return Unauthorized(new { message = "User not authenticated" });
            }

            _logger.LogInfo("Controller", $"UpdateCourseProgress: userId={userId}, courseId={courseId}, progress={dto.Progress}");
            await _employeeService.UpdateCourseProgressAsync(userId, courseId, dto.Progress);
            return Ok(new { message = "Progress updated" });
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"UpdateCourseProgress failed: courseId={courseId}", ex);
            return StatusCode(500, new { message = "Error updating progress" });
        }
    }

    [HttpPost("courses/{courseId}/certificate/request")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> RequestCourseCertificate(int courseId)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value 
                ?? User.FindFirst("id")?.Value 
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Controller", "GenerateCertificate: User not authenticated");
                return Unauthorized(new { message = "User not authenticated" });
            }

            _logger.LogInfo("Controller", $"RequestCourseCertificate: userId={userId}, courseId={courseId}");
            var request = await _employeeService.RequestCourseCertificateAsync(userId, courseId);
            _logger.LogInfo("Controller", $"Certificate request submitted: userId={userId}, courseId={courseId}");
            return Ok(request);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"RequestCourseCertificate failed: courseId={courseId}", ex);
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("learning-paths/{learningPathId}/certificate/request")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> RequestLearningPathCertificate(int learningPathId)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value 
                ?? User.FindFirst("id")?.Value 
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Controller", "GenerateLearningPathCertificate: User not authenticated");
                return Unauthorized(new { message = "User not authenticated" });
            }

            _logger.LogInfo("Controller", $"RequestLearningPathCertificate: userId={userId}, learningPathId={learningPathId}");
            var request = await _employeeService.RequestLearningPathCertificateAsync(userId, learningPathId);
            _logger.LogInfo("Controller", $"Learning path certificate request submitted: userId={userId}, learningPathId={learningPathId}");
            return Ok(request);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"RequestLearningPathCertificate failed: learningPathId={learningPathId}", ex);
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("modules/{courseId}/quiz")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> GetCourseQuiz(int courseId)
    {
        try
        {
            _logger.LogInfo("Controller", $"GetCourseQuiz: courseId={courseId}");
            var response = await new HttpClient().GetAsync($"https://localhost:7028/api/admin/quizzes/course/{courseId}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound(new { message = "No quiz found for this course" });
            }
            var quiz = await response.Content.ReadAsStringAsync();
            return Content(quiz, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"GetCourseQuiz failed: courseId={courseId}", ex);
            return StatusCode(500, new { message = "Error retrieving quiz" });
        }
    }

    [HttpGet("learning-paths/{learningPathId}/final-quiz")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> GetLearningPathFinalQuiz(int learningPathId, [FromServices] IQuizService quizService)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value 
                ?? User.FindFirst("id")?.Value 
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            _logger.LogInfo("Controller", $"GetLearningPathFinalQuiz: userId={userId}, learningPathId={learningPathId}");
            var quiz = await quizService.GetLearningPathFinalQuizAsync(learningPathId, userId);
            
            if (quiz == null)
            {
                return NotFound(new { message = "Final quiz not available. Complete all courses first." });
            }
            
            return Ok(quiz);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"GetLearningPathFinalQuiz failed: learningPathId={learningPathId}", ex);
            return StatusCode(500, new { message = "Error retrieving final quiz" });
        }
    }

    [HttpPost("quizzes/{quizId}/submit")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> SubmitQuiz(int quizId, [FromBody] object dto)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value 
                ?? User.FindFirst("id")?.Value 
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            _logger.LogInfo("Controller", $"SubmitQuiz: userId={userId}, quizId={quizId}");
            var client = new HttpClient();
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"https://localhost:7028/api/admin/quizzes/{quizId}/submit", content);
            var result = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, result);
            }
            
            return Content(result, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"SubmitQuiz failed: quizId={quizId}", ex);
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("certificates")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> GetMyCertificates([FromServices] learning_path_tracker.Database.AppDbContext context)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value 
                ?? User.FindFirst("id")?.Value 
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            _logger.LogInfo("Controller", $"GetMyCertificates: userId={userId}");
            
            var certificates = await context.Certificates
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.IssuedAt)
                .Select(c => new
                {
                    c.Id,
                    c.CertificateId,
                    c.EmployeeName,
                    c.CourseName,
                    c.LearningPathName,
                    c.ManagerName,
                    c.AverageScore,
                    c.IssuedAt,
                    c.CertificateType
                })
                .ToListAsync();
            
            return Ok(certificates);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetMyCertificates failed", ex);
            return StatusCode(500, new { message = "Error retrieving certificates" });
        }
    }

    [HttpGet("certificates/{certificateId}/download")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> DownloadCertificate(int certificateId, 
        [FromServices] learning_path_tracker.Database.AppDbContext context,
        [FromServices] learning_path_tracker.Api.Services.CertificateGeneratorService certService)
    {
        try
        {
            var certificate = await context.Certificates.FindAsync(certificateId);
            if (certificate == null)
                return NotFound(new { message = "Certificate not found" });

            var pdfBytes = certService.GenerateCertificatePdf(
                certificate.CertificateId,
                certificate.EmployeeName,
                certificate.CourseName ?? "",
                certificate.LearningPathName ?? "",
                certificate.ManagerName ?? "",
                certificate.AverageScore,
                certificate.IssuedAt,
                certificate.CertificateType
            );

            return File(pdfBytes, "application/pdf", $"Certificate_{certificate.CertificateId}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"DownloadCertificate failed: certificateId={certificateId}", ex);
            return StatusCode(500, new { message = "Error downloading certificate" });
        }
    }

    [HttpGet("certificates/{certificateId}/preview")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> PreviewCertificate(int certificateId, 
        [FromServices] learning_path_tracker.Database.AppDbContext context,
        [FromServices] learning_path_tracker.Api.Services.CertificateGeneratorService certService)
    {
        try
        {
            var certificate = await context.Certificates.FindAsync(certificateId);
            if (certificate == null)
                return NotFound(new { message = "Certificate not found" });

            var pdfBytes = certService.GenerateCertificatePdf(
                certificate.CertificateId,
                certificate.EmployeeName,
                certificate.CourseName ?? "",
                certificate.LearningPathName ?? "",
                certificate.ManagerName ?? "",
                certificate.AverageScore,
                certificate.IssuedAt,
                certificate.CertificateType
            );

            return File(pdfBytes, "application/pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"PreviewCertificate failed: certificateId={certificateId}", ex);
            return StatusCode(500, new { message = "Error previewing certificate" });
        }
    }

    [HttpGet("{userId}/certificates")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> GetUserCertificates(int userId, [FromServices] learning_path_tracker.Database.AppDbContext context)
    {
        try
        {
            _logger.LogInfo("Controller", $"GetUserCertificates: userId={userId}");
            
            var certificates = await context.Certificates
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.IssuedAt)
                .Select(c => new
                {
                    c.Id,
                    c.CertificateId,
                    c.EmployeeName,
                    c.CourseName,
                    c.LearningPathName,
                    c.ManagerName,
                    c.AverageScore,
                    c.IssuedAt,
                    c.CertificateType
                })
                .ToListAsync();
            
            return Ok(certificates);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"GetUserCertificates failed: userId={userId}", ex);
            return StatusCode(500, new { message = "Error retrieving certificates" });
        }
    }

    [HttpGet("{userId}/progress")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> GetProgress(int userId, [FromServices] learning_path_tracker.Database.AppDbContext context)
    {
        try
        {
            _logger.LogInfo("Controller", $"GetProgress: userId={userId}");
            
            var enrollments = await context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.Course)
                .ToListAsync();
            
            var assignments = await context.Assignments
                .Where(a => a.EmployeeId == userId)
                .Include(a => a.LearningPath)
                .ToListAsync();
            
            var totalModules = enrollments.Count;
            var completedModules = enrollments.Count(e => 
                e.Progress >= 80 && (!e.Course.QuizId.HasValue || e.QuizPassed));
            
            var totalMinutes = 0;
            foreach (var enrollment in enrollments)
            {
                double totalVideoMins = 0;
                
                if (!string.IsNullOrEmpty(enrollment.Course.VideoDuration))
                {
                    var duration = enrollment.Course.VideoDuration;
                    var parts = duration.Split(':');
                    if (parts.Length >= 2 && int.TryParse(parts[0], out int mins) && int.TryParse(parts[1], out int secs))
                    {
                        totalVideoMins = mins + (secs / 60.0);
                    }
                }
                else if (enrollment.Course.DurationHours > 0)
                {
                    totalVideoMins = enrollment.Course.DurationHours;
                }
                
                if (totalVideoMins > 0)
                {
                    var watchedMins = (totalVideoMins * enrollment.Progress) / 100.0;
                    totalMinutes += (int)Math.Round(watchedMins);
                }
            }
            
            var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7028/api/admin/learning-paths/with-courses");
            var json = await response.Content.ReadAsStringAsync();
            var learningPathsData = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(json);
            
            var pathsProgress = new List<object>();
            foreach (var assignment in assignments)
            {
                var lpData = learningPathsData?.FirstOrDefault(lp => lp.GetProperty("id").GetInt32() == assignment.LearningPathId);
                if (lpData == null) continue;
                
                var courses = lpData.Value.GetProperty("courses").EnumerateArray().ToList();
                var pathTotal = courses.Count;
                
                var pathCourseIds = new List<int>();
                foreach (var course in courses)
                {
                    var courseId = 0;
                    if (course.TryGetProperty("courseId", out var courseIdProp))
                        courseId = courseIdProp.GetInt32();
                    else if (course.TryGetProperty("id", out var idProp))
                        courseId = idProp.GetInt32();
                    
                    if (courseId > 0)
                        pathCourseIds.Add(courseId);
                }
                
                var pathEnrollments = enrollments.Where(e => pathCourseIds.Contains(e.CourseId)).ToList();
                var pathCompleted = pathEnrollments.Count(e => 
                    e.Progress >= 80 && (!e.Course.QuizId.HasValue || e.QuizPassed));
                var pathProgress = pathTotal > 0 ? (pathCompleted * 100 / pathTotal) : 0;
                
                pathsProgress.Add(new {
                    name = assignment.LearningPath.Title,
                    totalModules = pathTotal,
                    completedModules = pathCompleted,
                    progress = pathProgress
                });
            }
            
            return Ok(new {
                totalModules,
                completedModules,
                totalHours = totalMinutes,
                paths = pathsProgress
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"GetProgress failed: userId={userId}", ex);
            return StatusCode(500, new { message = "Error retrieving progress" });
        }
    }

    [HttpPost("learning-paths/{learningPathId}/courses/{courseId}/certificate/request")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> RequestCourseInPathCertificate(int learningPathId, int courseId)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value 
                ?? User.FindFirst("id")?.Value 
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            _logger.LogInfo("Controller", $"RequestCourseInPathCertificate: userId={userId}, learningPathId={learningPathId}, courseId={courseId}");
            var request = await _employeeService.RequestCourseCertificateAsync(userId, courseId);
            _logger.LogInfo("Controller", $"Course certificate request submitted: userId={userId}, courseId={courseId}");
            return Ok(request);
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", $"RequestCourseInPathCertificate failed: learningPathId={learningPathId}, courseId={courseId}", ex);
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("learning-schedule")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> GetLearningSchedule([FromServices] learning_path_tracker.Database.AppDbContext context)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value ?? User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "User not authenticated" });

            var schedule = await context.LearningSchedules.FirstOrDefaultAsync(s => s.UserId == userId);
            if (schedule == null)
                return Ok(new { startTime = "", endTime = "", isActive = false });

            return Ok(new {
                id = schedule.Id,
                startTime = schedule.StartTime.ToString(@"hh\:mm"),
                endTime = schedule.EndTime.ToString(@"hh\:mm"),
                isActive = schedule.IsActive
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "GetLearningSchedule failed", ex);
            return StatusCode(500, new { message = "Error retrieving schedule" });
        }
    }

    [HttpPost("learning-schedule")]
    [AuthorizeRoles("Employee", "Manager", "Admin")]
    public async Task<IActionResult> SetLearningSchedule([FromBody] learning_path_tracker.Application.DTOs.SetLearningScheduleDto dto, 
        [FromServices] learning_path_tracker.Database.AppDbContext context)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value ?? User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "User not authenticated" });

            if (!TimeSpan.TryParse(dto.StartTime, out var startTime) || !TimeSpan.TryParse(dto.EndTime, out var endTime))
                return BadRequest(new { message = "Invalid time format" });

            var schedule = await context.LearningSchedules.FirstOrDefaultAsync(s => s.UserId == userId);
            if (schedule == null)
            {
                schedule = new learning_path_tracker.Domain.Entities.LearningSchedule
                {
                    UserId = userId,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                context.LearningSchedules.Add(schedule);
            }
            else
            {
                schedule.StartTime = startTime;
                schedule.EndTime = endTime;
                schedule.IsActive = true;
                schedule.UpdatedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
            return Ok(new { message = "Schedule saved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError("Controller", "SetLearningSchedule failed", ex);
            return StatusCode(500, new { message = "Error saving schedule" });
        }
    }
}


