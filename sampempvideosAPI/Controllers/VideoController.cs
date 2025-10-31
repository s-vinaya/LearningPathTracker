using EMPBACKEND.Interfaces;
using EMPBACKEND.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Video>>> GetAllVideos()
        {
            var videos = await _videoService.GetAllVideosAsync();
            return Ok(videos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Video>> GetVideo(int id)
        {
            var video = await _videoService.GetVideoByIdAsync(id);
            if (video == null)
                return NotFound();

            return Ok(video);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<Video>>> GetVideosByCourse(int courseId)
        {
            var videos = await _videoService.GetVideosByCourseAsync(courseId);
            return Ok(videos);
        }

        [HttpPost]
        public async Task<ActionResult<Video>> CreateVideo(Video video)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdVideo = await _videoService.CreateVideoAsync(video);
            return CreatedAtAction(nameof(GetVideo), new { id = createdVideo.Id }, createdVideo);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Video>> UpdateVideo(int id, Video video)
        {
            if (id != video.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedVideo = await _videoService.UpdateVideoAsync(video);
                return Ok(updatedVideo);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVideo(int id)
        {
            var result = await _videoService.DeleteVideoAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}