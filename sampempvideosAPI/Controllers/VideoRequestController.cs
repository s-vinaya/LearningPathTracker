using EMPBACKEND.Interfaces;
using EMPBACKEND.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoRequestController : ControllerBase
    {
        private readonly IVideoRequestService _requestService;

        public VideoRequestController(IVideoRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VideoRequest>>> GetAllRequests()
        {
            var requests = await _requestService.GetAllRequestsAsync();
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VideoRequest>> GetRequest(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);
            if (request == null)
                return NotFound();

            return Ok(request);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<VideoRequest>>> GetUserRequests(int userId)
        {
            var requests = await _requestService.GetUserRequestsAsync(userId);
            return Ok(requests);
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<VideoRequest>>> GetPendingRequests()
        {
            var requests = await _requestService.GetPendingRequestsAsync();
            return Ok(requests);
        }

        [HttpPost]
        public async Task<ActionResult<VideoRequest>> CreateRequest(VideoRequest videoRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdRequest = await _requestService.CreateRequestAsync(videoRequest);
            return CreatedAtAction(nameof(GetRequest), new { id = createdRequest.Id }, createdRequest);
        }

        [HttpPut("{id}/approve")]
        public async Task<ActionResult<VideoRequest>> ApproveRequest(int id)
        {
            try
            {
                var approvedRequest = await _requestService.ApproveRequestAsync(id);
                return Ok(approvedRequest);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/reject")]
        public async Task<ActionResult<VideoRequest>> RejectRequest(int id)
        {
            try
            {
                var rejectedRequest = await _requestService.RejectRequestAsync(id);
                return Ok(rejectedRequest);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }
    }
}