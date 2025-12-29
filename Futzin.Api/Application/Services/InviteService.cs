using System.Security.Cryptography;
using Futzin.Api.Application.DTOs;
using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Interfaces;

namespace Futzin.Api.Application.Services;

public class InviteService
{
    private readonly IInviteRepository _inviteRepository;
    private readonly IPeladaRepository _peladaRepository;
    private readonly IUserRepository _userRepository;

    public InviteService(
        IInviteRepository inviteRepository,
        IPeladaRepository peladaRepository,
        IUserRepository userRepository)
    {
        _inviteRepository = inviteRepository;
        _peladaRepository = peladaRepository;
        _userRepository = userRepository;
    }

    public string GenerateInviteToken()
    {
        var randomBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return Convert.ToBase64String(randomBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public string GetInviteLink(Pelada pelada, string baseUrl)
    {
        return $"{baseUrl}/join/{pelada.InviteToken}";
    }

    public async Task<InviteInfoDto> InviteUserAsync(int peladaId, string email, int currentUserId)
    {
        var pelada = await _peladaRepository.GetByIdAsync(peladaId);
        if (pelada == null)
        {
            throw new InvalidOperationException("Pelada não encontrada");
        }

        if (pelada.CreatedById != currentUserId)
        {
            throw new UnauthorizedAccessException("Apenas o criador pode convidar usuários");
        }

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            throw new InvalidOperationException($"Usuário com email '{email}' não encontrado");
        }

        var existingInvite = await _inviteRepository.GetByPeladaAndUserAsync(peladaId, user.Id);
        if (existingInvite != null)
        {
            throw new InvalidOperationException("Usuário já foi convidado para esta pelada");
        }

        var invite = new PeladaInvite
        {
            PeladaId = peladaId,
            InvitedUserId = user.Id,
            IsAccepted = false
        };

        var createdInvite = await _inviteRepository.CreateAsync(invite);

        return new InviteInfoDto
        {
            Id = createdInvite.Id,
            UserId = user.Id,
            UserName = user.Name,
            UserEmail = user.Email,
            PhotoUrl = user.PhotoUrl,
            IsAccepted = createdInvite.IsAccepted,
            InvitedAt = createdInvite.InvitedAt,
            AcceptedAt = createdInvite.AcceptedAt
        };
    }

    public async Task<bool> RemoveInviteAsync(int peladaId, int inviteId, int currentUserId)
    {
        var pelada = await _peladaRepository.GetByIdAsync(peladaId);
        if (pelada == null)
        {
            return false;
        }

        if (pelada.CreatedById != currentUserId)
        {
            throw new UnauthorizedAccessException("Apenas o criador pode remover convites");
        }

        var invite = await _inviteRepository.GetByIdAsync(inviteId);
        if (invite == null || invite.PeladaId != peladaId)
        {
            return false;
        }

        await _inviteRepository.DeleteAsync(inviteId);
        return true;
    }

    public async Task<IEnumerable<InviteInfoDto>> GetInvitesByPeladaAsync(int peladaId, int currentUserId)
    {
        var pelada = await _peladaRepository.GetByIdAsync(peladaId);
        if (pelada == null)
        {
            throw new InvalidOperationException("Pelada não encontrada");
        }

        if (pelada.CreatedById != currentUserId)
        {
            throw new UnauthorizedAccessException("Apenas o criador pode ver os convites");
        }

        var invites = await _inviteRepository.GetByPeladaIdAsync(peladaId);

        return invites.Select(i => new InviteInfoDto
        {
            Id = i.Id,
            UserId = i.InvitedUserId,
            UserName = i.InvitedUser?.Name ?? "Unknown",
            UserEmail = i.InvitedUser?.Email ?? "Unknown",
            PhotoUrl = i.InvitedUser?.PhotoUrl,
            IsAccepted = i.IsAccepted,
            InvitedAt = i.InvitedAt,
            AcceptedAt = i.AcceptedAt
        });
    }

    public async Task<bool> CanUserJoinAsync(int peladaId, int userId, string? inviteToken = null)
    {
        var pelada = await _peladaRepository.GetByIdAsync(peladaId);
        if (pelada == null)
        {
            return false;
        }

        if (pelada.CreatedById == userId)
        {
            return true;
        }

        if (pelada.IsPublic)
        {
            return true;
        }

        if (!string.IsNullOrEmpty(inviteToken) && inviteToken == pelada.InviteToken)
        {
            return true;
        }

        var isInvited = await _inviteRepository.IsUserInvitedAsync(peladaId, userId);
        return isInvited;
    }

    public async Task AcceptInviteAsync(int peladaId, int userId)
    {
        var invite = await _inviteRepository.GetByPeladaAndUserAsync(peladaId, userId);
        if (invite != null && !invite.IsAccepted)
        {
            invite.IsAccepted = true;
            invite.AcceptedAt = DateTime.UtcNow;
            await _inviteRepository.UpdateAsync(invite);
        }
    }
}
