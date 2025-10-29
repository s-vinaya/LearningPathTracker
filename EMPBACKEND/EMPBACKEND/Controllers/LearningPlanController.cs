using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearningPlanController : ControllerBase
    {
        private readonly ILearningPlanService _service;

        public LearningPlanController(ILearningPlanService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LearningPlanDto>>> GetAll()
        {
            var learningPlans = await _service.GetAllAsync();
            return Ok(learningPlans);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LearningPlanDto>> GetById(int id)
        {
            var learningPlan = await _service.GetByIdAsync(id);
            if (learningPlan == null) return NotFound();
            return Ok(learningPlan);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<LearningPlanDto>>> GetByUserId(int userId)
        {
            var learningPlans = await _service.GetByUserIdAsync(userId);
            return Ok(learningPlans);
        }

        [HttpPost]
        public async Task<ActionResult<LearningPlanDto>> Create(CreateLearningPlanDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var learningPlan = await _service.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = learningPlan.Id }, learningPlan);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LearningPlanDto>> Update(int id, UpdateLearningPlanDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var learningPlan = await _service.UpdateAsync(id, updateDto);
                return Ok(learningPlan);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}