using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Interfaces;
using Futzin.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Futzin.Api.Infrastructure.Repositories;

public class ParticipantRepository : IParticipantRepository
{
    private readonly FutzinDbContext _context;

    public ParticipantRepository(FutzinDbContext context)
    {
        _context = context;
    }

    public async Task<PeladaParticipant?> GetByIdAsync(int id)
    {
        return await _context.PeladaParticipants
            .Include(pp => pp.User)
            .Include(pp => pp.Pelada)
            .Include(pp => pp.Team)
            .FirstOrDefaultAsync(pp => pp.Id == id);
    }

    public async Task<PeladaParticipant?> GetByPeladaAndUserAsync(int peladaId, int userId)
    {
        return await _context.PeladaParticipants
            .Include(pp => pp.User)
            .Include(pp => pp.Pelada)
            .Include(pp => pp.Team)
            .FirstOrDefaultAsync(pp => pp.PeladaId == peladaId && pp.UserId == userId);
    }

    public async Task<IEnumerable<PeladaParticipant>> GetByPeladaIdAsync(int peladaId)
    {
        return await _context.PeladaParticipants
            .Include(pp => pp.User)
            .Include(pp => pp.Team)
            .Where(pp => pp.PeladaId == peladaId)
            .OrderBy(pp => pp.JoinedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PeladaParticipant>> GetByUserIdAsync(int userId)
    {
        return await _context.PeladaParticipants
            .Include(pp => pp.Pelada)
                .ThenInclude(p => p.CreatedBy)
            .Include(pp => pp.Team)
            .Where(pp => pp.UserId == userId)
            .OrderByDescending(pp => pp.Pelada.Date)
            .ToListAsync();
    }

    public async Task<PeladaParticipant> AddAsync(PeladaParticipant participant)
    {
        participant.JoinedAt = DateTime.UtcNow;
        _context.PeladaParticipants.Add(participant);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(participant.Id) ?? participant;
    }

    public async Task<PeladaParticipant> UpdateAsync(PeladaParticipant participant)
    {
        _context.PeladaParticipants.Update(participant);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(participant.Id) ?? participant;
    }

    public async Task RemoveAsync(int id)
    {
        var participant = await _context.PeladaParticipants.FindAsync(id);
        if (participant != null)
        {
            _context.PeladaParticipants.Remove(participant);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsParticipatingAsync(int peladaId, int userId)
    {
        return await _context.PeladaParticipants
            .AnyAsync(pp => pp.PeladaId == peladaId && pp.UserId == userId);
    }

    public async Task<int> GetCountAsync(int peladaId)
    {
        return await _context.PeladaParticipants
            .CountAsync(pp => pp.PeladaId == peladaId);
    }
}
