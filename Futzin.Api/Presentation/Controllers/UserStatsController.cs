using Microsoft.AspNetCore.Mvc;
using Futzin.Api.Application.Services;

namespace Futzin.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserStatsController : ControllerBase
{
    private readonly UserStatsService _service;

    public UserStatsController(UserStatsService service)
    {
        _service = service;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserStats(int userId)
    {
        var stats = await _service.GetUserStatsAsync(userId);
        if (stats == null)
            return NotFound();

        return Ok(stats);
    }

    [HttpPost("{userId}/recalculate")]
    public async Task<IActionResult> RecalculateStats(int userId)
    {
        await _service.RecalculateUserStatsAsync(userId);
        return NoContent();
    }
}
