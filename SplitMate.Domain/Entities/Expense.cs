namespace SplitMate.Domain.Entities;

public class Expense
{
    public Guid Id { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public Guid PaidByUserId { get; private set; }
    public Guid GroupId { get; private set; }
    public DateTime Date { get; private set; }

    public User PaidBy { get; private set; } = null!;
    public Group Group { get; private set; } = null!;
    public ICollection<ExpenseSplit> Splits { get; private set; } = new List<ExpenseSplit>();

    protected Expense() { }

    public static Expense Create(string description, decimal amount, Guid paidByUserId, Guid groupId)
    {
        if (amount <= 0)
            throw new ArgumentException("O valor do gasto deve ser maior que zero.");

        return new Expense
        {
            Id = Guid.NewGuid(),
            Description = description,
            Amount = amount,
            PaidByUserId = paidByUserId,
            GroupId = groupId,
            Date = DateTime.UtcNow
        };
    }
}