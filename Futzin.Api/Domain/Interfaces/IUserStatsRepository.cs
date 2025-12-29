using Futzin.Api.Domain.Entities;

namespace Futzin.Api.Domain.Interfaces;

public interface IUserStatsRepository
{
    Task<UserStats?> GetByUserIdAsync(int userId);
    Task<UserStats> CreateAsync(UserStats stats);
    Task<UserStats> UpdateAsync(UserStats stats);
    Task RecalculateStatsAsync(int userId);
}
