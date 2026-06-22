namespace SplitMate.Domain.Services;

public class DebtTransaction
{
    public Guid FromUserId { get; init; }
    public Guid ToUserId { get; init; }
    public decimal Amount { get; init; }
}

public static class DebtCalculator
{
    public static IReadOnlyList<DebtTransaction> Calculate(
        IEnumerable<(Guid PaidByUserId, decimal Amount, IEnumerable<(Guid UserId, decimal Share)> Splits)> expenses)
    {
        var balances = new Dictionary<Guid, decimal>();

        foreach (var expense in expenses)
        {
            balances.TryAdd(expense.PaidByUserId, 0);
            balances[expense.PaidByUserId] += expense.Amount;

            foreach (var split in expense.Splits)
            {
                balances.TryAdd(split.UserId, 0);
                balances[split.UserId] -= split.Share;
            }
        }

        var creditors = new Queue<(Guid Id, decimal Amount)>(
            balances.Where(b => b.Value > 0).Select(b => (b.Key, b.Value)));

        var debtors = new Queue<(Guid Id, decimal Amount)>(
            balances.Where(b => b.Value < 0).Select(b => (b.Key, Math.Abs(b.Value))));

        var transactions = new List<DebtTransaction>();

        while (creditors.Count > 0 && debtors.Count > 0)
        {
            var creditor = creditors.Dequeue();
            var debtor = debtors.Dequeue();

            var amount = Math.Min(creditor.Amount, debtor.Amount);

            transactions.Add(new DebtTransaction
            {
                FromUserId = debtor.Id,
                ToUserId = creditor.Id,
                Amount = Math.Round(amount, 2)
            });

            var creditorRemainder = creditor.Amount - amount;
            var debtorRemainder = debtor.Amount - amount;

            if (creditorRemainder > 0.01m)
                creditors.Enqueue((creditor.Id, creditorRemainder));

            if (debtorRemainder > 0.01m)
                debtors.Enqueue((debtor.Id, debtorRemainder));
        }

        return transactions;
    }
}