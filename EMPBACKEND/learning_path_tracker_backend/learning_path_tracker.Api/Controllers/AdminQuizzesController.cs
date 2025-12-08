using learning_path_tracker.Application.DTOs.Quizzes;
using learning_path_tracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/admin/quizzes")]
public class AdminQuizzesController : ControllerBase
{
    private readonly IQuizService _quizService;

    public AdminQuizzesController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var quizzes = await _quizService.GetAllQuizzesAsync();
        return Ok(quizzes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var quiz = await _quizService.GetQuizByIdAsync(id);
        if (quiz == null) return NotFound();
        return Ok(quiz);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuizDto dto)
    {
        var quiz = await _quizService.CreateQuizAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = quiz.Id }, quiz);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuizDto dto)
    {
        var quiz = await _quizService.UpdateQuizAsync(id, dto);
        if (quiz == null) return NotFound();
        return Ok(quiz);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _quizService.DeleteQuizAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpGet("course/{courseId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetQuizByCourseId(int courseId)
    {
        var quizzes = await _quizService.GetAllQuizzesAsync();
        var quiz = quizzes.FirstOrDefault(q => q.CourseId == courseId);
        if (quiz == null) return NotFound(new { message = "No quiz found for this course" });
        return Ok(quiz);
    }

    [HttpGet("{id}/check-attempts")]
    public async Task<IActionResult> CheckAttempts(int id)
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

            var result = await _quizService.CheckQuizAttemptsAsync(userId, id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> SubmitQuiz(int id, [FromBody] SubmitQuizDto dto)
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

            var result = await _quizService.SubmitQuizAttemptAsync(userId, id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
