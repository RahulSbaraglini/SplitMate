namespace SplitMate.Domain.Entities;

public class Settlement
{
    public Guid Id { get; private set; }
    public Guid GroupId { get; private set; }
    public Guid FromUserId { get; private set; }
    public Guid ToUserId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime PaidAt { get; private set; }

    public Group Group { get; private set; } = null!;
    public User FromUser { get; private set; } = null!;
    public User ToUser { get; private set; } = null!;

    protected Settlement() { }

    public static Settlement Create(Guid groupId, Guid fromUserId, Guid toUserId, decimal amount)
    {
        return new Settlement
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            FromUserId = fromUserId,
            ToUserId = toUserId,
            Amount = amount,
            PaidAt = DateTime.UtcNow
        };
    }
}