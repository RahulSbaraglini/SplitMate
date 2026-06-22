using SplitMate.Application.DTOs;
using SplitMate.Application.Interfaces;
using SplitMate.Domain.Entities;

namespace SplitMate.Application.UseCases.Groups;

public class CreateGroupUseCase
{
    private readonly IGroupRepository _groupRepository;

    public CreateGroupUseCase(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<GroupResponse> ExecuteAsync(CreateGroupRequest request, Guid userId)
    {
        // criamos o grupo passando o userId de quem está criando
        // o Domain gera o InviteCode automaticamente no método Create
        var group = Group.Create(request.Name, userId);

        // quem cria o grupo já entra como membro automaticamente — faz sentido no negócio
        var member = GroupMember.Create(group.Id, userId);
        group.Members.Add(member);

        await _groupRepository.AddAsync(group);

        return new GroupResponse(group.Id, group.Name, group.InviteCode, group.Members.Count, new List<GroupMemberResponse>());
    }
}