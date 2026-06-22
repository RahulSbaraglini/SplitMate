using SplitMate.Domain.Services;

namespace SplitMate.Tests.Domain;

public class DebtCalculatorTests
{
    [Fact]
    public void Calculate_WhenOnePersonPaidForTwo_ShouldReturnOneTransaction()
    {
        var ana = Guid.NewGuid();
        var bruno = Guid.NewGuid();

        var expenses = new[]
        {
            (
                PaidByUserId: ana,
                Amount: 100m,
                Splits: new (Guid UserId, decimal Share)[]
                {
                    (ana, 50m),
                    (bruno, 50m)
                }.AsEnumerable()
            )
        };

        var result = DebtCalculator.Calculate(expenses);

        Assert.Single(result);
        Assert.Equal(bruno, result[0].FromUserId);
        Assert.Equal(ana, result[0].ToUserId);
        Assert.Equal(50m, result[0].Amount);
    }

    [Fact]
    public void Calculate_WhenThreePeople_ShouldMinimizeTransactions()
    {
        var ana = Guid.NewGuid();
        var bruno = Guid.NewGuid();
        var carla = Guid.NewGuid();

        var expenses = new[]
        {
            (
                PaidByUserId: ana,
                Amount: 90m,
                Splits: new (Guid UserId, decimal Share)[]
                {
                    (ana, 30m),
                    (bruno, 30m),
                    (carla, 30m)
                }.AsEnumerable()
            )
        };

        var result = DebtCalculator.Calculate(expenses);

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(ana, t.ToUserId));
        Assert.All(result, t => Assert.Equal(30m, t.Amount));
    }

    [Fact]
    public void Calculate_WhenDebtsCancel_ShouldReturnNoTransactions()
    {
        var ana = Guid.NewGuid();
        var bruno = Guid.NewGuid();

        var expenses = new[]
        {
            (
                PaidByUserId: ana,
                Amount: 100m,
                Splits: new (Guid UserId, decimal Share)[]
                {
                    (ana, 50m),
                    (bruno, 50m)
                }.AsEnumerable()
            ),
            (
                PaidByUserId: bruno,
                Amount: 100m,
                Splits: new (Guid UserId, decimal Share)[]
                {
                    (ana, 50m),
                    (bruno, 50m)
                }.AsEnumerable()
            )
        };

        var result = DebtCalculator.Calculate(expenses);

        Assert.Empty(result);
    }

    [Fact]
    public void Calculate_WhenExpenseHasNegativeAmount_ShouldThrowException()
    {
        var ana = Guid.NewGuid();
        var bruno = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
        {
            var expense = SplitMate.Domain.Entities.Expense.Create("Teste", -50m, ana, Guid.NewGuid());
        });
    }
}