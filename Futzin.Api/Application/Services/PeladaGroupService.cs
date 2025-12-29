using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Interfaces;

namespace Futzin.Api.Application.Services;

public class PeladaGroupService
{
    private readonly IPeladaGroupRepository _repository;

    public PeladaGroupService(IPeladaGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task<PeladaGroup?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<PeladaGroup>> GetUserGroupsAsync(int userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    public async Task<PeladaGroup> CreateGroupAsync(string name, string? description, int createdById)
    {
        var group = new PeladaGroup
        {
            Name = name,
            Description = description,
            CreatedById = createdById
        };

        var createdGroup = await _repository.CreateAsync(group);

        await _repository.AddMemberAsync(new GroupMember
        {
            GroupId = createdGroup.Id,
            UserId = createdById,
            IsAdmin = true
        });

        return createdGroup;
    }

    public async Task<PeladaGroup> UpdateGroupAsync(int id, string name, string? description)
    {
        var group = await _repository.GetByIdAsync(id);
        if (group == null)
            throw new Exception("Grupo não encontrado");

        group.Name = name;
        group.Description = description;

        return await _repository.UpdateAsync(group);
    }

    public async Task DeleteGroupAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<GroupMember> AddMemberAsync(int groupId, int userId, bool isAdmin = false)
    {
        var isMember = await _repository.IsUserMemberAsync(groupId, userId);
        if (isMember)
            throw new Exception("Usuário já é membro do grupo");

        return await _repository.AddMemberAsync(new GroupMember
        {
            GroupId = groupId,
            UserId = userId,
            IsAdmin = isAdmin
        });
    }

    public async Task RemoveMemberAsync(int groupId, int userId)
    {
        await _repository.RemoveMemberAsync(groupId, userId);
    }
}
