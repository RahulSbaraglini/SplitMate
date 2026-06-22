namespace SplitMate.Domain.Entities;

public class GroupMember
{
    public Guid GroupId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public Group Group { get; private set; } = null!;
    public User User { get; private set; } = null!;

    protected GroupMember() { }

    public static GroupMember Create(Guid groupId, Guid userId)
    {
        return new GroupMember
        {
            GroupId = groupId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        };
    }
}