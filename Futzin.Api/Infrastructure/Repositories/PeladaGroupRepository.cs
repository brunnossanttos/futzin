using Microsoft.EntityFrameworkCore;
using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Interfaces;
using Futzin.Api.Infrastructure.Data;

namespace Futzin.Api.Infrastructure.Repositories;

public class PeladaGroupRepository : IPeladaGroupRepository
{
    private readonly FutzinDbContext _context;

    public PeladaGroupRepository(FutzinDbContext context)
    {
        _context = context;
    }

    public async Task<PeladaGroup?> GetByIdAsync(int id)
    {
        return await _context.PeladaGroups
            .Include(g => g.Members)
            .ThenInclude(m => m.User)
            .Include(g => g.CreatedBy)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<PeladaGroup>> GetByUserIdAsync(int userId)
    {
        return await _context.PeladaGroups
            .Include(g => g.Members)
            .ThenInclude(m => m.User)
            .Where(g => g.Members.Any(m => m.UserId == userId) && g.IsActive)
            .ToListAsync();
    }

    public async Task<PeladaGroup> CreateAsync(PeladaGroup group)
    {
        _context.PeladaGroups.Add(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task<PeladaGroup> UpdateAsync(PeladaGroup group)
    {
        group.UpdatedAt = DateTime.UtcNow;
        _context.PeladaGroups.Update(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task DeleteAsync(int id)
    {
        var group = await _context.PeladaGroups.FindAsync(id);
        if (group != null)
        {
            group.IsActive = false;
            group.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsUserMemberAsync(int groupId, int userId)
    {
        return await _context.GroupMembers
            .AnyAsync(m => m.GroupId == groupId && m.UserId == userId);
    }

    public async Task<GroupMember> AddMemberAsync(GroupMember member)
    {
        _context.GroupMembers.Add(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task RemoveMemberAsync(int groupId, int userId)
    {
        var member = await _context.GroupMembers
            .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);

        if (member != null)
        {
            _context.GroupMembers.Remove(member);
            await _context.SaveChangesAsync();
        }
    }
}
