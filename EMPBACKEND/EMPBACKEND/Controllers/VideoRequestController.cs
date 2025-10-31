using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VideoRequestController : ControllerBase
    {
        private readonly IVideoRequestService _service;

        public VideoRequestController(IVideoRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> GetAllVideoRequests()
        {
            try
            {
                var videoRequests = await _service.GetAllVideoRequestsAsync();
                return Ok(videoRequests);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetVideoRequest(int id)
        {
            try
            {
                var videoRequest = await _service.GetVideoRequestByIdAsync(id);
                if (videoRequest == null) return NotFound();
                return Ok(videoRequest);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetVideoRequestsByUser(int userId)
        {
            try
            {
                var videoRequests = await _service.GetVideoRequestsByUserIdAsync(userId);
                return Ok(videoRequests);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> GetVideoRequestsByStatus(string status)
        {
            try
            {
                var videoRequests = await _service.GetVideoRequestsByStatusAsync(status);
                return Ok(videoRequests);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateVideoRequest([FromBody] CreateVideoRequestDto dto)
        {
            try
            {
                var createdVideoRequest = await _service.CreateVideoRequestAsync(dto);
                return CreatedAtAction(nameof(GetVideoRequest), new { id = createdVideoRequest.Id }, createdVideoRequest);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateVideoRequest(int id, [FromBody] UpdateVideoRequestDto dto)
        {
            try
            {
                if (id != dto.Id) return BadRequest("ID mismatch");
                var updatedVideoRequest = await _service.UpdateVideoRequestAsync(dto);
                return Ok(updatedVideoRequest);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> UpdateVideoRequestStatus(int id, [FromBody] string status)
        {
            try
            {
                var updatedVideoRequest = await _service.UpdateVideoRequestStatusAsync(id, status);
                return Ok(updatedVideoRequest);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteVideoRequest(int id)
        {
            try
            {
                var result = await _service.DeleteVideoRequestAsync(id);
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