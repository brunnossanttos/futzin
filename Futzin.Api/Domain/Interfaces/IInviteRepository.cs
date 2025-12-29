using Futzin.Api.Domain.Entities;

namespace Futzin.Api.Domain.Interfaces;

public interface IInviteRepository
{
    Task<PeladaInvite?> GetByIdAsync(int id);

    Task<IEnumerable<PeladaInvite>> GetByPeladaIdAsync(int peladaId);

    Task<PeladaInvite?> GetByPeladaAndUserAsync(int peladaId, int userId);

    Task<PeladaInvite> CreateAsync(PeladaInvite invite);

    Task<PeladaInvite> UpdateAsync(PeladaInvite invite);

    Task DeleteAsync(int id);

    Task<bool> IsUserInvitedAsync(int peladaId, int userId);
}
