using Microsoft.EntityFrameworkCore;
using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Enums;
using Futzin.Api.Domain.Interfaces;
using Futzin.Api.Infrastructure.Data;

namespace Futzin.Api.Infrastructure.Repositories;

public class UserStatsRepository : IUserStatsRepository
{
    private readonly FutzinDbContext _context;

    public UserStatsRepository(FutzinDbContext context)
    {
        _context = context;
    }

    public async Task<UserStats?> GetByUserIdAsync(int userId)
    {
        return await _context.UserStats
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<UserStats> CreateAsync(UserStats stats)
    {
        _context.UserStats.Add(stats);
        await _context.SaveChangesAsync();
        return stats;
    }

    public async Task<UserStats> UpdateAsync(UserStats stats)
    {
        stats.UpdatedAt = DateTime.UtcNow;
        _context.UserStats.Update(stats);
        await _context.SaveChangesAsync();
        return stats;
    }

    public async Task RecalculateStatsAsync(int userId)
    {
        var stats = await GetByUserIdAsync(userId);
        if (stats == null)
        {
            stats = new UserStats { UserId = userId };
            await CreateAsync(stats);
        }

        var attendances = await _context.PeladaAttendances
            .Include(a => a.Pelada)
            .Where(a => a.UserId == userId &&
                   (a.Status == AttendanceStatus.Attended || a.Status == AttendanceStatus.Confirmed))
            .ToListAsync();

        stats.TotalPeladas = attendances.Count(a => a.Status == AttendanceStatus.Attended);

        stats.TotalGoals = await _context.Goals
            .Where(g => g.ScorerId == userId)
            .CountAsync();

        stats.TotalNoShows = await _context.PeladaAttendances
            .Where(a => a.UserId == userId && a.Status == AttendanceStatus.NoShow)
            .CountAsync();

        if (stats.TotalPeladas > 0)
        {
            stats.GoalsPerGame = (decimal)stats.TotalGoals / stats.TotalPeladas;
            stats.WinRate = stats.TotalWins > 0 ? (decimal)stats.TotalWins / stats.TotalPeladas * 100 : 0;
        }

        var totalInvites = await _context.PeladaAttendances
            .Where(a => a.UserId == userId)
            .CountAsync();

        if (totalInvites > 0)
        {
            var attended = await _context.PeladaAttendances
                .Where(a => a.UserId == userId && a.Status == AttendanceStatus.Attended)
                .CountAsync();

            stats.AttendanceRate = (decimal)attended / totalInvites * 100;
        }

        var lastPelada = await _context.PeladaAttendances
            .Include(a => a.Pelada)
            .Where(a => a.UserId == userId && a.Status == AttendanceStatus.Attended)
            .OrderByDescending(a => a.Pelada.Date)
            .FirstOrDefaultAsync();

        stats.LastPeladaDate = lastPelada?.Pelada.Date;

        await UpdateAsync(stats);
    }
}
