using Microsoft.AspNetCore.Mvc;
using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DailyGoalController : ControllerBase
    {
        private readonly IDailyGoalService _service;

        public DailyGoalController(IDailyGoalService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DailyGoalDto>>> GetAll()
        {
            var goals = await _service.GetAllAsync();
            return Ok(goals);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DailyGoalDto>> GetById(int id)
        {
            var goal = await _service.GetByIdAsync(id);
            return goal == null ? NotFound() : Ok(goal);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<DailyGoalDto>>> GetByUserId(int userId)
        {
            var goals = await _service.GetByUserIdAsync(userId);
            return Ok(goals);
        }

        [HttpGet("user/{userId}/date/{date}")]
        public async Task<ActionResult<IEnumerable<DailyGoalDto>>> GetByUserIdAndDate(int userId, DateTime date)
        {
            var goals = await _service.GetByUserIdAndDateAsync(userId, date);
            return Ok(goals);
        }

        [HttpPost]
        public async Task<ActionResult<DailyGoalDto>> Create(CreateDailyGoalDto createDto)
        {
            var goal = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = goal.Id }, goal);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DailyGoalDto>> Update(int id, UpdateDailyGoalDto updateDto)
        {
            var goal = await _service.UpdateAsync(id, updateDto);
            return Ok(goal);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}