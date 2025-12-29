using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Interfaces;

namespace Futzin.Api.Application.Services;

public class UserStatsService
{
    private readonly IUserStatsRepository _repository;

    public UserStatsService(IUserStatsRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserStats?> GetUserStatsAsync(int userId)
    {
        var stats = await _repository.GetByUserIdAsync(userId);
        if (stats == null)
        {
            stats = new UserStats { UserId = userId };
            await _repository.CreateAsync(stats);
        }

        return stats;
    }

    public async Task RecalculateUserStatsAsync(int userId)
    {
        await _repository.RecalculateStatsAsync(userId);
    }
}
