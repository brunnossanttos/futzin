using Futzin.Api.Application.DTOs;
using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Interfaces;

namespace Futzin.Api.Application.Services;

public class TeamService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IPeladaRepository _peladaRepository;
    private readonly IParticipantRepository _participantRepository;

    public TeamService(
        ITeamRepository teamRepository,
        IPeladaRepository peladaRepository,
        IParticipantRepository participantRepository)
    {
        _teamRepository = teamRepository;
        _peladaRepository = peladaRepository;
        _participantRepository = participantRepository;
    }

    public async Task<IEnumerable<TeamInfoDto>> GenerateTeamsAsync(int peladaId, int currentUserId)
    {
        var pelada = await _peladaRepository.GetByIdWithDetailsAsync(peladaId);
        if (pelada == null)
        {
            throw new InvalidOperationException("Pelada não encontrada");
        }

        if (pelada.CreatedById != currentUserId)
        {
            throw new UnauthorizedAccessException("Apenas o criador pode sortear times");
        }

        var participants = pelada.Participants.ToList();
        if (participants.Count < 2)
        {
            throw new InvalidOperationException("É necessário pelo menos 2 participantes para sortear times");
        }

        if (participants.Count % 2 != 0)
        {
            throw new InvalidOperationException("Número de participantes deve ser par para dividir em 2 times");
        }

        await _teamRepository.DeleteByPeladaIdAsync(peladaId);

        var team1 = new Team
        {
            Name = "Time 1",
            Color = "#FF0000", // Red
            PeladaId = peladaId
        };

        var team2 = new Team
        {
            Name = "Time 2",
            Color = "#0000FF", // Blue
            PeladaId = peladaId
        };

        var createdTeams = (await _teamRepository.CreateRangeAsync(new[] { team1, team2 })).ToList();
        var createdTeam1 = createdTeams[0];
        var createdTeam2 = createdTeams[1];

        var shuffledParticipants = ShuffleList(participants);

        var halfCount = shuffledParticipants.Count / 2;

        for (int i = 0; i < shuffledParticipants.Count; i++)
        {
            var participant = shuffledParticipants[i];
            participant.TeamId = i < halfCount ? createdTeam1.Id : createdTeam2.Id;
            await _participantRepository.UpdateAsync(participant);
        }

        var finalTeams = await _teamRepository.GetByPeladaIdAsync(peladaId);

        return finalTeams.Select(t => new TeamInfoDto
        {
            Id = t.Id,
            Name = t.Name,
            Color = t.Color,
            PlayerCount = t.Players?.Count ?? 0,
            PlayerNames = t.Players?.Select(p => p.User?.Name ?? "Unknown").ToList() ?? new List<string>()
        });
    }

    public async Task<IEnumerable<TeamInfoDto>> GetTeamsByPeladaAsync(int peladaId)
    {
        var teams = await _teamRepository.GetByPeladaIdAsync(peladaId);

        return teams.Select(t => new TeamInfoDto
        {
            Id = t.Id,
            Name = t.Name,
            Color = t.Color,
            PlayerCount = t.Players?.Count ?? 0,
            PlayerNames = t.Players?.Select(p => p.User?.Name ?? "Unknown").ToList() ?? new List<string>()
        });
    }

    public async Task ClearTeamsAsync(int peladaId, int currentUserId)
    {
        var pelada = await _peladaRepository.GetByIdAsync(peladaId);
        if (pelada == null)
        {
            throw new InvalidOperationException("Pelada não encontrada");
        }

        if (pelada.CreatedById != currentUserId)
        {
            throw new UnauthorizedAccessException("Apenas o criador pode limpar times");
        }

        var participants = await _participantRepository.GetByPeladaIdAsync(peladaId);
        foreach (var participant in participants)
        {
            participant.TeamId = null;
            await _participantRepository.UpdateAsync(participant);
        }

        await _teamRepository.DeleteByPeladaIdAsync(peladaId);
    }

    #region Private Helper Methods

    private List<T> ShuffleList<T>(List<T> list)
    {
        var random = new Random();
        var shuffled = new List<T>(list);

        int n = shuffled.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (shuffled[k], shuffled[n]) = (shuffled[n], shuffled[k]);
        }

        return shuffled;
    }

    #endregion
}
