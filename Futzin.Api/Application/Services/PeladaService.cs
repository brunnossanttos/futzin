using Futzin.Api.Application.DTOs;
using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Interfaces;

namespace Futzin.Api.Application.Services;

public class PeladaService
{
    private readonly IPeladaRepository _peladaRepository;
    private readonly IUserRepository _userRepository;
    private readonly InviteService _inviteService;

    public PeladaService(
        IPeladaRepository peladaRepository,
        IUserRepository userRepository,
        InviteService inviteService)
    {
        _peladaRepository = peladaRepository;
        _userRepository = userRepository;
        _inviteService = inviteService;
    }

    public async Task<IEnumerable<PeladaResponseDto>> GetAllAsync()
    {
        var peladas = await _peladaRepository.GetAllAsync();
        return peladas.Select(MapToPeladaResponseDto);
    }

    public async Task<IEnumerable<PeladaResponseDto>> GetActiveAsync()
    {
        var peladas = await _peladaRepository.GetActiveAsync();
        return peladas.Select(MapToPeladaResponseDto);
    }

    public async Task<IEnumerable<PeladaResponseDto>> GetUpcomingAsync()
    {
        var peladas = await _peladaRepository.GetUpcomingAsync();
        return peladas.Select(MapToPeladaResponseDto);
    }

    public async Task<IEnumerable<PeladaResponseDto>> GetByCreatorIdAsync(int userId)
    {
        var peladas = await _peladaRepository.GetByCreatorIdAsync(userId);
        return peladas.Select(MapToPeladaResponseDto);
    }

    public async Task<IEnumerable<PeladaResponseDto>> GetByParticipantIdAsync(int userId)
    {
        var peladas = await _peladaRepository.GetByParticipantIdAsync(userId);
        return peladas.Select(MapToPeladaResponseDto);
    }

    public async Task<PeladaResponseDto?> GetByIdAsync(int id)
    {
        var pelada = await _peladaRepository.GetByIdAsync(id);
        return pelada != null ? MapToPeladaResponseDto(pelada) : null;
    }

    public async Task<PeladaDetailDto?> GetDetailByIdAsync(int id)
    {
        var pelada = await _peladaRepository.GetByIdWithDetailsAsync(id);
        return pelada != null ? MapToPeladaDetailDto(pelada) : null;
    }

    public async Task<PeladaResponseDto> CreateAsync(CreatePeladaDto dto, int createdById)
    {
        var creator = await _userRepository.GetByIdAsync(createdById);
        if (creator == null)
        {
            throw new InvalidOperationException("Usuário criador não encontrado");
        }

        var pelada = new Pelada
        {
            Name = dto.Name,
            Description = dto.Description,
            Date = dto.Date,
            Location = dto.Location,
            FieldType = dto.FieldType,
            Price = dto.Price,
            MaxPlayers = dto.MaxPlayers,
            CreatedById = createdById,
            IsActive = true,
            IsPublic = dto.IsPublic,
            InviteToken = _inviteService.GenerateInviteToken()
        };

        var createdPelada = await _peladaRepository.CreateAsync(pelada);
        return MapToPeladaResponseDto(createdPelada);
    }

    public async Task<PeladaResponseDto?> UpdateAsync(int id, UpdatePeladaDto dto, int userId)
    {
        var pelada = await _peladaRepository.GetByIdAsync(id);

        if (pelada == null)
        {
            return null;
        }

        if (pelada.CreatedById != userId)
        {
            throw new UnauthorizedAccessException("Apenas o criador pode atualizar a pelada");
        }

        if (dto.Name != null) pelada.Name = dto.Name;
        if (dto.Description != null) pelada.Description = dto.Description;
        if (dto.Date.HasValue) pelada.Date = dto.Date.Value;
        if (dto.Location != null) pelada.Location = dto.Location;
        if (dto.FieldType.HasValue) pelada.FieldType = dto.FieldType.Value;
        if (dto.Price.HasValue) pelada.Price = dto.Price.Value;

        if (dto.MaxPlayers.HasValue)
        {
            var currentCount = await _peladaRepository.GetParticipantCountAsync(id);
            if (dto.MaxPlayers.Value < currentCount)
            {
                throw new InvalidOperationException(
                    $"Não é possível reduzir o número de jogadores para {dto.MaxPlayers.Value}. " +
                    $"Já existem {currentCount} participantes confirmados");
            }
            pelada.MaxPlayers = dto.MaxPlayers.Value;
        }

        if (dto.IsActive.HasValue) pelada.IsActive = dto.IsActive.Value;

        var updatedPelada = await _peladaRepository.UpdateAsync(pelada);
        return MapToPeladaResponseDto(updatedPelada);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var pelada = await _peladaRepository.GetByIdWithDetailsAsync(id);

        if (pelada == null)
        {
            return false;
        }

        if (pelada.CreatedById != userId)
        {
            throw new UnauthorizedAccessException("Apenas o criador pode deletar a pelada");
        }

        var hasParticipantsPaid = pelada.Participants.Any(p => p.HasPaid);
        if (hasParticipantsPaid)
        {
            throw new InvalidOperationException(
                "Não é possível deletar uma pelada com participantes que já pagaram. " +
                "Cancele a pelada (IsActive = false) ao invés de deletar");
        }

        await _peladaRepository.DeleteAsync(id);
        return true;
    }

    #region Private Mapping Methods

    private PeladaResponseDto MapToPeladaResponseDto(Pelada pelada)
    {
        return new PeladaResponseDto
        {
            Id = pelada.Id,
            Name = pelada.Name,
            Description = pelada.Description,
            Date = pelada.Date,
            Location = pelada.Location,
            FieldType = pelada.FieldType,
            FieldTypeName = pelada.FieldType.ToString(),
            Price = pelada.Price,
            MaxPlayers = pelada.MaxPlayers,
            CurrentPlayers = pelada.Participants?.Count ?? 0,
            IsActive = pelada.IsActive,
            IsPublic = pelada.IsPublic,
            CreatedById = pelada.CreatedById,
            CreatedByName = pelada.CreatedBy?.Name ?? "Unknown",
            CreatedAt = pelada.CreatedAt,
            UpdatedAt = pelada.UpdatedAt
        };
    }

    private PeladaDetailDto MapToPeladaDetailDto(Pelada pelada)
    {
        var currentPlayers = pelada.Participants.Count;

        return new PeladaDetailDto
        {
            Id = pelada.Id,
            Name = pelada.Name,
            Description = pelada.Description,
            Date = pelada.Date,
            Location = pelada.Location,
            FieldType = pelada.FieldType,
            FieldTypeName = pelada.FieldType.ToString(),
            Price = pelada.Price,
            MaxPlayers = pelada.MaxPlayers,
            CurrentPlayers = currentPlayers,
            IsFull = currentPlayers >= pelada.MaxPlayers,
            IsActive = pelada.IsActive,
            IsPublic = pelada.IsPublic,
            InviteLink = null,
            CreatedById = pelada.CreatedById,
            CreatedByName = pelada.CreatedBy?.Name ?? "Unknown",
            CreatedAt = pelada.CreatedAt,
            UpdatedAt = pelada.UpdatedAt,
            Participants = pelada.Participants.Select(p => new ParticipantInfoDto
            {
                UserId = p.UserId,
                UserName = p.User?.Name ?? "Unknown",
                PhotoUrl = p.User?.PhotoUrl,
                HasPaid = p.HasPaid,
                TeamId = p.TeamId,
                TeamName = p.Team?.Name,
                JoinedAt = p.JoinedAt
            }).ToList(),
            Teams = pelada.Teams.Select(t => new TeamInfoDto
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color,
                PlayerCount = t.Players?.Count ?? 0,
                PlayerNames = t.Players?.Select(p => p.User?.Name ?? "Unknown").ToList() ?? new List<string>()
            }).ToList(),
            Invites = pelada.Invites.Select(i => new InviteInfoDto
            {
                Id = i.Id,
                UserId = i.InvitedUserId,
                UserName = i.InvitedUser?.Name ?? "Unknown",
                UserEmail = i.InvitedUser?.Email ?? "Unknown",
                PhotoUrl = i.InvitedUser?.PhotoUrl,
                IsAccepted = i.IsAccepted,
                InvitedAt = i.InvitedAt,
                AcceptedAt = i.AcceptedAt
            }).ToList()
        };
    }

    #endregion
}
