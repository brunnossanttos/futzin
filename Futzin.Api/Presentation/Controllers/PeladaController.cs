using Futzin.Api.Application.DTOs;
using Futzin.Api.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Futzin.Api.Presentation.Controllers;

[ApiController]
[Route("api/peladas")]
[Authorize]
public class PeladaController : ControllerBase
{
    private readonly PeladaService _peladaService;
    private readonly ParticipantService _participantService;

    public PeladaController(
        PeladaService peladaService,
        ParticipantService participantService)
    {
        _peladaService = peladaService;
        _participantService = participantService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PeladaResponseDto>>> GetAll()
    {
        try
        {
            var peladas = await _peladaService.GetAllAsync();
            return Ok(peladas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar peladas", error = ex.Message });
        }
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PeladaResponseDto>>> GetActive()
    {
        try
        {
            var peladas = await _peladaService.GetActiveAsync();
            return Ok(peladas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar peladas ativas", error = ex.Message });
        }
    }

    [HttpGet("upcoming")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PeladaResponseDto>>> GetUpcoming()
    {
        try
        {
            var peladas = await _peladaService.GetUpcomingAsync();
            return Ok(peladas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar próximas peladas", error = ex.Message });
        }
    }

    [HttpGet("my-peladas")]
    public async Task<ActionResult<IEnumerable<PeladaResponseDto>>> GetMyPeladas()
    {
        try
        {
            var userId = GetCurrentUserId();
            var peladas = await _peladaService.GetByCreatorIdAsync(userId);
            return Ok(peladas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar suas peladas", error = ex.Message });
        }
    }

    [HttpGet("my-participations")]
    public async Task<ActionResult<IEnumerable<PeladaResponseDto>>> GetMyParticipations()
    {
        try
        {
            var userId = GetCurrentUserId();
            var peladas = await _peladaService.GetByParticipantIdAsync(userId);
            return Ok(peladas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar suas participações", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PeladaResponseDto>> GetById(int id)
    {
        try
        {
            var pelada = await _peladaService.GetByIdAsync(id);

            if (pelada == null)
            {
                return NotFound(new { message = "Pelada não encontrada" });
            }

            return Ok(pelada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar pelada", error = ex.Message });
        }
    }

    [HttpGet("{id}/details")]
    [AllowAnonymous]
    public async Task<ActionResult<PeladaDetailDto>> GetDetailById(int id)
    {
        try
        {
            var pelada = await _peladaService.GetDetailByIdAsync(id);

            if (pelada == null)
            {
                return NotFound(new { message = "Pelada não encontrada" });
            }

            return Ok(pelada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar detalhes da pelada", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<PeladaResponseDto>> Create([FromBody] CreatePeladaDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var pelada = await _peladaService.CreateAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = pelada.Id },
                pelada);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao criar pelada", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PeladaResponseDto>> Update(int id, [FromBody] UpdatePeladaDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var pelada = await _peladaService.UpdateAsync(id, dto, userId);

            if (pelada == null)
            {
                return NotFound(new { message = "Pelada não encontrada" });
            }

            return Ok(pelada);
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
            return StatusCode(500, new { message = "Erro ao atualizar pelada", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _peladaService.DeleteAsync(id, userId);

            if (!success)
            {
                return NotFound(new { message = "Pelada não encontrada" });
            }

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
            return StatusCode(500, new { message = "Erro ao deletar pelada", error = ex.Message });
        }
    }

    [HttpPost("{id}/join")]
    public async Task<ActionResult<ParticipantInfoDto>> JoinPelada(int id, [FromBody] JoinWithTokenDto? dto = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            var inviteToken = dto?.InviteToken;
            var participant = await _participantService.JoinPeladaAsync(id, userId, inviteToken);
            return Ok(participant);
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
            return StatusCode(500, new { message = "Erro ao entrar na pelada", error = ex.Message });
        }
    }

    [HttpPost("{id}/leave")]
    public async Task<ActionResult> LeavePelada(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _participantService.LeavePeladaAsync(id, userId);

            if (!success)
            {
                return NotFound(new { message = "Você não está participando desta pelada" });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao sair da pelada", error = ex.Message });
        }
    }

    [HttpGet("{id}/participants")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ParticipantInfoDto>>> GetParticipants(int id)
    {
        try
        {
            var participants = await _participantService.GetParticipantsByPeladaAsync(id);
            return Ok(participants);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar participantes", error = ex.Message });
        }
    }

    [HttpPatch("{peladaId}/participants/{participantUserId}/payment")]
    public async Task<ActionResult<ParticipantInfoDto>> UpdatePaymentStatus(
        int peladaId,
        int participantUserId,
        [FromBody] UpdatePaymentStatusDto dto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var participant = await _participantService.UpdatePaymentStatusAsync(
                peladaId,
                participantUserId,
                dto.HasPaid,
                currentUserId);

            if (participant == null)
            {
                return NotFound(new { message = "Participante não encontrado" });
            }

            return Ok(participant);
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
            return StatusCode(500, new { message = "Erro ao atualizar status de pagamento", error = ex.Message });
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
