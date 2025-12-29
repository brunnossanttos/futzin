using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Interfaces;
using Futzin.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Futzin.Api.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly FutzinDbContext _context;

    public TeamRepository(FutzinDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByIdAsync(int id)
    {
        return await _context.Teams
            .Include(t => t.Players)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Team>> GetByPeladaIdAsync(int peladaId)
    {
        return await _context.Teams
            .Include(t => t.Players)
                .ThenInclude(p => p.User)
            .Where(t => t.PeladaId == peladaId)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Team> CreateAsync(Team team)
    {
        team.CreatedAt = DateTime.UtcNow;
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(team.Id) ?? team;
    }

    public async Task<IEnumerable<Team>> CreateRangeAsync(IEnumerable<Team> teams)
    {
        var teamList = teams.ToList();
        foreach (var team in teamList)
        {
            team.CreatedAt = DateTime.UtcNow;
        }

        _context.Teams.AddRange(teamList);
        await _context.SaveChangesAsync();

        return teamList;
    }

    public async Task DeleteByPeladaIdAsync(int peladaId)
    {
        var teams = await _context.Teams
            .Where(t => t.PeladaId == peladaId)
            .ToListAsync();

        if (teams.Any())
        {
            _context.Teams.RemoveRange(teams);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var team = await _context.Teams.FindAsync(id);
        if (team != null)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }
    }
}
