using Futzin.Api.Application.DTOs;
using Futzin.Api.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Futzin.Api.Presentation.Controllers;

[ApiController]
[Route("api/teams")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly TeamService _teamService;

    public TeamsController(TeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet("pelada/{peladaId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<TeamInfoDto>>> GetTeams(int peladaId)
    {
        try
        {
            var teams = await _teamService.GetTeamsByPeladaAsync(peladaId);
            return Ok(teams);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar times", error = ex.Message });
        }
    }

    [HttpPost("pelada/{peladaId}/generate")]
    public async Task<ActionResult<IEnumerable<TeamInfoDto>>> GenerateTeams(int peladaId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var teams = await _teamService.GenerateTeamsAsync(peladaId, userId);
            return Ok(teams);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao gerar times", error = ex.Message });
        }
    }

    [HttpDelete("pelada/{peladaId}")]
    public async Task<ActionResult> ClearTeams(int peladaId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _teamService.ClearTeamsAsync(peladaId, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao limpar times", error = ex.Message });
        }
    }

    #region Helper Methods

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    #endregion
}
