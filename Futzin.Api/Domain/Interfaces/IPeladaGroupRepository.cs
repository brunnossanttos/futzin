using Futzin.Api.Domain.Entities;

namespace Futzin.Api.Domain.Interfaces;

public interface IPeladaGroupRepository
{
    Task<PeladaGroup?> GetByIdAsync(int id);
    Task<IEnumerable<PeladaGroup>> GetByUserIdAsync(int userId);
    Task<PeladaGroup> CreateAsync(PeladaGroup group);
    Task<PeladaGroup> UpdateAsync(PeladaGroup group);
    Task DeleteAsync(int id);
    Task<bool> IsUserMemberAsync(int groupId, int userId);
    Task<GroupMember> AddMemberAsync(GroupMember member);
    Task RemoveMemberAsync(int groupId, int userId);
}
