using Futzin.Api.Domain.Entities;

namespace Futzin.Api.Domain.Interfaces;

public interface IPeladaRepository
{
    Task<Pelada?> GetByIdAsync(int id);

    Task<Pelada?> GetByIdWithDetailsAsync(int id);

    Task<IEnumerable<Pelada>> GetAllAsync();

    Task<IEnumerable<Pelada>> GetActiveAsync();

    Task<IEnumerable<Pelada>> GetUpcomingAsync();

    Task<IEnumerable<Pelada>> GetByCreatorIdAsync(int userId);

    Task<IEnumerable<Pelada>> GetByParticipantIdAsync(int userId);

    Task<Pelada> CreateAsync(Pelada pelada);

    Task<Pelada> UpdateAsync(Pelada pelada);

    Task DeleteAsync(int id);

    Task<bool> ExistsAsync(int id);
    Task<int> GetParticipantCountAsync(int peladaId);

    Task<bool> IsFullAsync(int peladaId);
}
