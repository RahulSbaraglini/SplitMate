namespace SplitMate.Application.DTOs;

// DTO de saída do grupo — note que devolvemos o InviteCode
// é ele que os amigos vão usar pra entrar no grupo
public record GroupMemberResponse(Guid UserId, string Name);

public record GroupResponse(
    Guid Id,
    string Name,
    string InviteCode,
    int MemberCount,
    IEnumerable<GroupMemberResponse> Members
);