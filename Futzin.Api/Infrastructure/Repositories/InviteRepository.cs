using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Interfaces;
using Futzin.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Futzin.Api.Infrastructure.Repositories;

public class InviteRepository : IInviteRepository
{
    private readonly FutzinDbContext _context;

    public InviteRepository(FutzinDbContext context)
    {
        _context = context;
    }

    public async Task<PeladaInvite?> GetByIdAsync(int id)
    {
        return await _context.PeladaInvites
            .Include(i => i.InvitedUser)
            .Include(i => i.Pelada)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<PeladaInvite>> GetByPeladaIdAsync(int peladaId)
    {
        return await _context.PeladaInvites
            .Include(i => i.InvitedUser)
            .Where(i => i.PeladaId == peladaId)
            .OrderByDescending(i => i.InvitedAt)
            .ToListAsync();
    }

    public async Task<PeladaInvite?> GetByPeladaAndUserAsync(int peladaId, int userId)
    {
        return await _context.PeladaInvites
            .Include(i => i.InvitedUser)
            .Include(i => i.Pelada)
            .FirstOrDefaultAsync(i => i.PeladaId == peladaId && i.InvitedUserId == userId);
    }

    public async Task<PeladaInvite> CreateAsync(PeladaInvite invite)
    {
        invite.InvitedAt = DateTime.UtcNow;
        _context.PeladaInvites.Add(invite);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(invite.Id) ?? invite;
    }

    public async Task<PeladaInvite> UpdateAsync(PeladaInvite invite)
    {
        _context.PeladaInvites.Update(invite);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(invite.Id) ?? invite;
    }

    public async Task DeleteAsync(int id)
    {
        var invite = await _context.PeladaInvites.FindAsync(id);
        if (invite != null)
        {
            _context.PeladaInvites.Remove(invite);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsUserInvitedAsync(int peladaId, int userId)
    {
        return await _context.PeladaInvites
            .AnyAsync(i => i.PeladaId == peladaId && i.InvitedUserId == userId);
    }
}
