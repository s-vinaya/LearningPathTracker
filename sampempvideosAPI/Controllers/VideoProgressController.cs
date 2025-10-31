using EMPBACKEND.Interfaces;
using EMPBACKEND.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoProgressController : ControllerBase
    {
        private readonly IVideoProgressService _progressService;

        public VideoProgressController(IVideoProgressService progressService)
        {
            _progressService = progressService;
        }

        [HttpGet("user/{userId}/video/{videoId}")]
        public async Task<ActionResult<VideoProgress>> GetProgress(int userId, int videoId)
        {
            var progress = await _progressService.GetProgressAsync(userId, videoId);
            if (progress == null)
                return NotFound();

            return Ok(progress);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<VideoProgress>>> GetUserProgress(int userId)
        {
            var progress = await _progressService.GetUserProgressAsync(userId);
            return Ok(progress);
        }

        [HttpPost("update")]
        public async Task<ActionResult<VideoProgress>> UpdateProgress([FromBody] UpdateProgressRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var progress = await _progressService.UpdateProgressAsync(request.UserId, request.VideoId, request.WatchedDuration);
                return Ok(progress);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("complete")]
        public async Task<ActionResult<VideoProgress>> MarkComplete([FromBody] CompleteVideoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var progress = await _progressService.MarkVideoCompleteAsync(request.UserId, request.VideoId);
                return Ok(progress);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("course/{courseId}/user/{userId}/progress")]
        public async Task<ActionResult<decimal>> GetCourseProgress(int userId, int courseId)
        {
            var progress = await _progressService.GetCourseProgressAsync(userId, courseId);
            return Ok(new { CourseId = courseId, UserId = userId, ProgressPercentage = progress });
        }
    }

    public class UpdateProgressRequest
    {
        public int UserId { get; set; }
        public int VideoId { get; set; }
        public int WatchedDuration { get; set; }
    }

    public class CompleteVideoRequest
    {
        public int UserId { get; set; }
        public int VideoId { get; set; }
    }
}