using Futzin.Api.Application.DTOs;
using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Interfaces;

namespace Futzin.Api.Application.Services;

public class ParticipantService
{
    private readonly IParticipantRepository _participantRepository;
    private readonly IPeladaRepository _peladaRepository;
    private readonly IUserRepository _userRepository;
    private readonly InviteService _inviteService;

    public ParticipantService(
        IParticipantRepository participantRepository,
        IPeladaRepository peladaRepository,
        IUserRepository userRepository,
        InviteService inviteService)
    {
        _participantRepository = participantRepository;
        _peladaRepository = peladaRepository;
        _userRepository = userRepository;
        _inviteService = inviteService;
    }

    public async Task<ParticipantInfoDto> JoinPeladaAsync(int peladaId, int userId, string? inviteToken = null)
    {
        var pelada = await _peladaRepository.GetByIdAsync(peladaId);
        if (pelada == null)
        {
            throw new InvalidOperationException("Pelada não encontrada");
        }

        if (!pelada.IsActive)
        {
            throw new InvalidOperationException("Pelada não está ativa");
        }

        if (pelada.Date < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Não é possível entrar em uma pelada que já aconteceu");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("Usuário não encontrado");
        }

        var canJoin = await _inviteService.CanUserJoinAsync(peladaId, userId, inviteToken);
        if (!canJoin)
        {
            throw new UnauthorizedAccessException(
                "Você não tem permissão para entrar nesta pelada. " +
                "A pelada é privada e você precisa de um convite ou link de acesso");
        }

        var isParticipating = await _participantRepository.IsParticipatingAsync(peladaId, userId);
        if (isParticipating)
        {
            throw new InvalidOperationException("Você já está participando desta pelada");
        }

        var isFull = await _peladaRepository.IsFullAsync(peladaId);
        if (isFull)
        {
            throw new InvalidOperationException("Pelada está cheia");
        }

        var participant = new PeladaParticipant
        {
            PeladaId = peladaId,
            UserId = userId,
            HasPaid = false
        };

        var addedParticipant = await _participantRepository.AddAsync(participant);

        await _inviteService.AcceptInviteAsync(peladaId, userId);

        return new ParticipantInfoDto
        {
            UserId = addedParticipant.UserId,
            UserName = addedParticipant.User?.Name ?? user.Name,
            PhotoUrl = addedParticipant.User?.PhotoUrl ?? user.PhotoUrl,
            HasPaid = addedParticipant.HasPaid,
            TeamId = addedParticipant.TeamId,
            TeamName = addedParticipant.Team?.Name,
            JoinedAt = addedParticipant.JoinedAt
        };
    }

    public async Task<bool> LeavePeladaAsync(int peladaId, int userId)
    {
        var participant = await _participantRepository.GetByPeladaAndUserAsync(peladaId, userId);

        if (participant == null)
        {
            return false;
        }

        var pelada = await _peladaRepository.GetByIdAsync(peladaId);
        if (pelada != null && pelada.Date < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Não é possível sair de uma pelada que já aconteceu");
        }

        if (participant.HasPaid)
        {
            throw new InvalidOperationException(
                "Você já pagou para esta pelada. Entre em contato com o organizador para cancelar");
        }

        await _participantRepository.RemoveAsync(participant.Id);
        return true;
    }

    public async Task<ParticipantInfoDto?> UpdatePaymentStatusAsync(
        int peladaId,
        int participantUserId,
        bool hasPaid,
        int currentUserId)
    {
        var pelada = await _peladaRepository.GetByIdAsync(peladaId);
        if (pelada == null)
        {
            throw new InvalidOperationException("Pelada não encontrada");
        }

        if (pelada.CreatedById != currentUserId)
        {
            throw new UnauthorizedAccessException(
                "Apenas o criador da pelada pode atualizar status de pagamento");
        }

        var participant = await _participantRepository.GetByPeladaAndUserAsync(peladaId, participantUserId);
        if (participant == null)
        {
            return null;
        }

        participant.HasPaid = hasPaid;
        var updatedParticipant = await _participantRepository.UpdateAsync(participant);

        return new ParticipantInfoDto
        {
            UserId = updatedParticipant.UserId,
            UserName = updatedParticipant.User?.Name ?? "Unknown",
            PhotoUrl = updatedParticipant.User?.PhotoUrl,
            HasPaid = updatedParticipant.HasPaid,
            TeamId = updatedParticipant.TeamId,
            TeamName = updatedParticipant.Team?.Name,
            JoinedAt = updatedParticipant.JoinedAt
        };
    }

    public async Task<IEnumerable<ParticipantInfoDto>> GetParticipantsByPeladaAsync(int peladaId)
    {
        var participants = await _participantRepository.GetByPeladaIdAsync(peladaId);

        return participants.Select(p => new ParticipantInfoDto
        {
            UserId = p.UserId,
            UserName = p.User?.Name ?? "Unknown",
            PhotoUrl = p.User?.PhotoUrl,
            HasPaid = p.HasPaid,
            TeamId = p.TeamId,
            TeamName = p.Team?.Name,
            JoinedAt = p.JoinedAt
        });
    }
}
