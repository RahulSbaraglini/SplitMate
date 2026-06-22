namespace SplitMate.Domain.Entities;

public class Group
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string InviteCode { get; private set; } = string.Empty;
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ICollection<GroupMember> Members { get; private set; } = new List<GroupMember>();
    public ICollection<Expense> Expenses { get; private set; } = new List<Expense>();

    protected Group() { }

    public static Group Create(string name, Guid createdBy)
    {
        return new Group
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            InviteCode = Guid.NewGuid().ToString("N")[..8].ToUpper()
        };
    }
}