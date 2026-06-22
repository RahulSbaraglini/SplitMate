namespace SplitMate.Domain.Entities;

public class ExpenseSplit
{
    public Guid ExpenseId { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }

    public Expense Expense { get; private set; } = null!;
    public User User { get; private set; } = null!;

    protected ExpenseSplit() { }

    public static ExpenseSplit Create(Guid expenseId, Guid userId, decimal amount)
    {
        return new ExpenseSplit
        {
            ExpenseId = expenseId,
            UserId = userId,
            Amount = amount
        };
    }
}