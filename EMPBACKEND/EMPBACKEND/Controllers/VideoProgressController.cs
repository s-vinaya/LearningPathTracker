using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VideoProgressController : ControllerBase
    {
        private readonly IVideoProgressService _service;

        public VideoProgressController(IVideoProgressService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> GetAllVideoProgress()
        {
            try
            {
                var videoProgress = await _service.GetAllVideoProgressAsync();
                return Ok(videoProgress);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetVideoProgress(int id)
        {
            try
            {
                var videoProgress = await _service.GetVideoProgressByIdAsync(id);
                if (videoProgress == null) return NotFound();
                return Ok(videoProgress);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetVideoProgressByUser(int userId)
        {
            try
            {
                var videoProgress = await _service.GetVideoProgressByUserIdAsync(userId);
                return Ok(videoProgress);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("video/{videoId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> GetVideoProgressByVideo(int videoId)
        {
            try
            {
                var videoProgress = await _service.GetVideoProgressByVideoIdAsync(videoId);
                return Ok(videoProgress);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user/{userId}/video/{videoId}")]
        public async Task<ActionResult> GetVideoProgressByUserAndVideo(int userId, int videoId)
        {
            try
            {
                var videoProgress = await _service.GetVideoProgressByUserAndVideoAsync(userId, videoId);
                if (videoProgress == null) return NotFound();
                return Ok(videoProgress);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateVideoProgress([FromBody] CreateVideoProgressDto dto)
        {
            try
            {
                var createdVideoProgress = await _service.CreateVideoProgressAsync(dto);
                return CreatedAtAction(nameof(GetVideoProgress), new { id = createdVideoProgress.Id }, createdVideoProgress);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateVideoProgress(int id, [FromBody] UpdateVideoProgressDto dto)
        {
            try
            {
                if (id != dto.Id) return BadRequest("ID mismatch");
                var updatedVideoProgress = await _service.UpdateVideoProgressAsync(dto);
                return Ok(updatedVideoProgress);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteVideoProgress(int id)
        {
            try
            {
                var result = await _service.DeleteVideoProgressAsync(id);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}