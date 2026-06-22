using SplitMate.Domain.Entities;

namespace SplitMate.Application.Interfaces;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(Guid id);
    Task<Group?> GetByInviteCodeAsync(string inviteCode);
    Task<IReadOnlyList<Group>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Group group);
    Task<bool> IsMemberAsync(Guid groupId, Guid userId);
    Task AddMemberAsync(GroupMember member);
}