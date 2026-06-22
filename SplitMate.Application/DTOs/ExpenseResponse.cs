namespace SplitMate.Application.DTOs;

public record ExpenseSplitResponse(Guid UserId, decimal Amount);

public record ExpenseResponse(
    Guid Id,
    string Description,
    decimal Amount,
    Guid PaidByUserId,
    string PaidByName,
    DateTime Date,
    IEnumerable<ExpenseSplitResponse> Splits
);