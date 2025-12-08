using learning_path_tracker.Api.Attributes;
using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformSettingsController : ControllerBase
{
    private readonly IPlatformSettingsService _settingsService;

    public PlatformSettingsController(IPlatformSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public async Task<ActionResult<PlatformSettingsDto>> GetSettings()
    {
        var settings = await _settingsService.GetSettingsAsync();
        return Ok(settings);
    }

    [HttpPut]
    [AuthorizeRoles("Admin")]
    public async Task<ActionResult<PlatformSettingsDto>> UpdateSettings([FromBody] PlatformSettingsDto dto)
    {
        var settings = await _settingsService.UpdateSettingsAsync(dto);
        return Ok(settings);
    }
}
