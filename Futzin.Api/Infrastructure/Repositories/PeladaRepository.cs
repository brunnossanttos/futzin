using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Interfaces;
using Futzin.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Futzin.Api.Infrastructure.Repositories;

public class PeladaRepository : IPeladaRepository
{
    private readonly FutzinDbContext _context;

    public PeladaRepository(FutzinDbContext context)
    {
        _context = context;
    }

    public async Task<Pelada?> GetByIdAsync(int id)
    {
        return await _context.Peladas
            .Include(p => p.CreatedBy)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Pelada?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Peladas
            .Include(p => p.CreatedBy)
            .Include(p => p.Participants)
                .ThenInclude(pp => pp.User)
            .Include(p => p.Participants)
                .ThenInclude(pp => pp.Team)
            .Include(p => p.Teams)
            .Include(p => p.Goals)
                .ThenInclude(g => g.Scorer)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Pelada>> GetAllAsync()
    {
        return await _context.Peladas
            .Include(p => p.CreatedBy)
            .Include(p => p.Participants)
            .OrderByDescending(p => p.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pelada>> GetActiveAsync()
    {
        return await _context.Peladas
            .Include(p => p.CreatedBy)
            .Include(p => p.Participants)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pelada>> GetUpcomingAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Peladas
            .Include(p => p.CreatedBy)
            .Include(p => p.Participants)
            .Where(p => p.IsActive && p.Date > now)
            .OrderBy(p => p.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pelada>> GetByCreatorIdAsync(int userId)
    {
        return await _context.Peladas
            .Include(p => p.CreatedBy)
            .Include(p => p.Participants)
            .Where(p => p.CreatedById == userId)
            .OrderByDescending(p => p.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pelada>> GetByParticipantIdAsync(int userId)
    {
        return await _context.Peladas
            .Include(p => p.CreatedBy)
            .Include(p => p.Participants)
            .Where(p => p.Participants.Any(pp => pp.UserId == userId))
            .OrderByDescending(p => p.Date)
            .ToListAsync();
    }

    public async Task<Pelada> CreateAsync(Pelada pelada)
    {
        pelada.CreatedAt = DateTime.UtcNow;
        _context.Peladas.Add(pelada);
        await _context.SaveChangesAsync();

        // Reload with CreatedBy to return complete entity
        return await GetByIdAsync(pelada.Id) ?? pelada;
    }

    public async Task<Pelada> UpdateAsync(Pelada pelada)
    {
        pelada.UpdatedAt = DateTime.UtcNow;
        _context.Peladas.Update(pelada);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(pelada.Id) ?? pelada;
    }

    public async Task DeleteAsync(int id)
    {
        var pelada = await _context.Peladas.FindAsync(id);
        if (pelada != null)
        {
            _context.Peladas.Remove(pelada);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Peladas.AnyAsync(p => p.Id == id);
    }

    public async Task<int> GetParticipantCountAsync(int peladaId)
    {
        return await _context.PeladaParticipants
            .CountAsync(pp => pp.PeladaId == peladaId);
    }

    public async Task<bool> IsFullAsync(int peladaId)
    {
        var pelada = await _context.Peladas.FindAsync(peladaId);
        if (pelada == null) return false;

        var participantCount = await GetParticipantCountAsync(peladaId);
        return participantCount >= pelada.MaxPlayers;
    }
}
