using Futzin.Api.Domain.Entities;

namespace Futzin.Api.Domain.Interfaces;

public interface IParticipantRepository
{
    Task<PeladaParticipant?> GetByIdAsync(int id);

    Task<PeladaParticipant?> GetByPeladaAndUserAsync(int peladaId, int userId);

    Task<IEnumerable<PeladaParticipant>> GetByPeladaIdAsync(int peladaId);

    Task<IEnumerable<PeladaParticipant>> GetByUserIdAsync(int userId);

    Task<PeladaParticipant> AddAsync(PeladaParticipant participant);

    Task<PeladaParticipant> UpdateAsync(PeladaParticipant participant);

    Task RemoveAsync(int id);

    Task<bool> IsParticipatingAsync(int peladaId, int userId);

    Task<int> GetCountAsync(int peladaId);
}
