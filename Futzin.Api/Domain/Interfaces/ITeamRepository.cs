using Futzin.Api.Domain.Entities;

namespace Futzin.Api.Domain.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(int id);

    Task<IEnumerable<Team>> GetByPeladaIdAsync(int peladaId);

    Task<Team> CreateAsync(Team team);

    Task<IEnumerable<Team>> CreateRangeAsync(IEnumerable<Team> teams);

    Task DeleteByPeladaIdAsync(int peladaId);

    Task DeleteAsync(int id);
}
