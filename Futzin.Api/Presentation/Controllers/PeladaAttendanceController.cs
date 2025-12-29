using Microsoft.AspNetCore.Mvc;
using Futzin.Api.Application.Services;
using Futzin.Api.Domain.Enums;

namespace Futzin.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeladaAttendanceController : ControllerBase
{
    private readonly PeladaAttendanceService _service;

    public PeladaAttendanceController(PeladaAttendanceService service)
    {
        _service = service;
    }

    [HttpGet("pelada/{peladaId}")]
    public async Task<IActionResult> GetPeladaAttendances(int peladaId)
    {
        var attendances = await _service.GetPeladaAttendancesAsync(peladaId);
        return Ok(attendances);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserAttendances(int userId)
    {
        var attendances = await _service.GetUserAttendancesAsync(userId);
        return Ok(attendances);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAttendance([FromBody] CreateAttendanceRequest request)
    {
        try
        {
            var attendance = await _service.CreateAttendanceAsync(request.PeladaId, request.UserId);
            return Ok(attendance);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{peladaId}/user/{userId}/confirm")]
    public async Task<IActionResult> ConfirmAttendance(int peladaId, int userId)
    {
        try
        {
            var attendance = await _service.ConfirmAttendanceAsync(peladaId, userId);
            return Ok(attendance);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{peladaId}/user/{userId}/decline")]
    public async Task<IActionResult> DeclineAttendance(int peladaId, int userId)
    {
        try
        {
            var attendance = await _service.DeclineAttendanceAsync(peladaId, userId);
            return Ok(attendance);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{peladaId}/user/{userId}/attended")]
    public async Task<IActionResult> MarkAttended(int peladaId, int userId)
    {
        try
        {
            await _service.MarkAttendedAsync(peladaId, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{peladaId}/user/{userId}/no-show")]
    public async Task<IActionResult> MarkNoShow(int peladaId, int userId)
    {
        try
        {
            await _service.MarkNoShowAsync(peladaId, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record CreateAttendanceRequest(int PeladaId, int UserId);
