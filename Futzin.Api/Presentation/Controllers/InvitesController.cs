using Futzin.Api.Application.DTOs;
using Futzin.Api.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Futzin.Api.Presentation.Controllers;

[ApiController]
[Route("api/peladas/{peladaId}/invites")]
[Authorize]
public class InvitesController : ControllerBase
{
    private readonly InviteService _inviteService;

    public InvitesController(InviteService inviteService)
    {
        _inviteService = inviteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InviteInfoDto>>> GetInvites(int peladaId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var invites = await _inviteService.GetInvitesByPeladaAsync(peladaId, userId);
            return Ok(invites);
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
            return StatusCode(500, new { message = "Erro ao buscar convites", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<InviteInfoDto>> InviteUser(int peladaId, [FromBody] InviteUserDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var invite = await _inviteService.InviteUserAsync(peladaId, dto.Email, userId);
            return Ok(invite);
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
            return StatusCode(500, new { message = "Erro ao convidar usuário", error = ex.Message });
        }
    }

    [HttpDelete("{inviteId}")]
    public async Task<ActionResult> RemoveInvite(int peladaId, int inviteId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _inviteService.RemoveInviteAsync(peladaId, inviteId, userId);

            if (!success)
            {
                return NotFound(new { message = "Convite não encontrado" });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao remover convite", error = ex.Message });
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
