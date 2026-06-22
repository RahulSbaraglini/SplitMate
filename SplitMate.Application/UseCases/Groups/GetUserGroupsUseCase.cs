using SplitMate.Application.DTOs;
using SplitMate.Application.Interfaces;

namespace SplitMate.Application.UseCases.Groups;

public class GetUserGroupsUseCase
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GetUserGroupsUseCase(IGroupRepository groupRepository, IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<GroupResponse>> ExecuteAsync(Guid userId)
    {
        var groups = await _groupRepository.GetByUserIdAsync(userId);

        var allUserIds = groups
            .SelectMany(g => g.Members.Select(m => m.UserId))
            .Distinct();

        var users = await _userRepository.GetByIdsAsync(allUserIds);
        var userDict = users.ToDictionary(u => u.Id, u => u.Name);

        // convertemos cada Group em GroupResponse — nunca devolvemos a entidade diretamente
        return groups.Select(g => new GroupResponse(
            g.Id,
            g.Name,
            g.InviteCode,
            g.Members.Count,
            g.Members.Select(m => new GroupMemberResponse(
                m.UserId,
                userDict.GetValueOrDefault(m.UserId, "Desconhecido")
            ))
        )).ToList();
    }
}