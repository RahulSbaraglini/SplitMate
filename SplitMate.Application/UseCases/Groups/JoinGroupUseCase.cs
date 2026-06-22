using SplitMate.Application.DTOs;
using SplitMate.Application.Interfaces;
using SplitMate.Domain.Entities;

namespace SplitMate.Application.UseCases.Groups;

public class JoinGroupUseCase
{
    private readonly IGroupRepository _groupRepository;

    public JoinGroupUseCase(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<GroupResponse> ExecuteAsync(JoinGroupRequest request, Guid userId)
    {
        // buscamos o grupo pelo código de convite
        var group = await _groupRepository.GetByInviteCodeAsync(request.InviteCode);

        if (group is null)
            throw new InvalidOperationException("Código de convite inválido.");

        // verificamos se o usuário já é membro — não faz sentido entrar duas vezes
        var alreadyMember = await _groupRepository.IsMemberAsync(group.Id, userId);
        if (alreadyMember)
            throw new InvalidOperationException("Você já é membro deste grupo.");

        var member = GroupMember.Create(group.Id, userId);
        await _groupRepository.AddMemberAsync(member);

        // buscamos novamente para ter o count de membros atualizado
        var updatedGroup = await _groupRepository.GetByIdAsync(group.Id);

        return new GroupResponse(updatedGroup!.Id, updatedGroup.Name, updatedGroup.InviteCode, updatedGroup.Members.Count, new List<GroupMemberResponse>());
    }
}