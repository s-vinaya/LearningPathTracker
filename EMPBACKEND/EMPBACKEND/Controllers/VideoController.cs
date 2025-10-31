using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllVideos()
        {
            try
            {
                var videos = await _videoService.GetAllVideoDtosAsync();
                return Ok(videos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetVideo(int id)
        {
            try
            {
                var video = await _videoService.GetVideoDtoByIdAsync(id);
                if (video == null) return NotFound();
                return Ok(video);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult> GetVideosByCourse(int courseId)
        {
            try
            {
                var videos = await _videoService.GetVideoDtosByCourseAsync(courseId);
                return Ok(videos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> CreateVideo([FromBody] CreateVideoDto dto)
        {
            try
            {
                var createdVideo = await _videoService.CreateVideoFromDtoAsync(dto);
                return CreatedAtAction(nameof(GetVideo), new { id = createdVideo.Id }, createdVideo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> UpdateVideo(int id, [FromBody] UpdateVideoDto dto)
        {
            try
            {
                if (id != dto.Id) return BadRequest("ID mismatch");
                var updatedVideo = await _videoService.UpdateVideoFromDtoAsync(id, dto);
                return Ok(updatedVideo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteVideo(int id)
        {
            try
            {
                await _videoService.DeleteAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}