namespace SplitMate.Application.DTOs;

// representa um item da divisão — quem participa e quanto deve
// usamos record porque é imutável e tem menos boilerplate — Clean Code
public record ExpenseSplitRequest(Guid UserId, decimal Amount);

public record AddExpenseRequest(
    string Description,
    decimal Amount,
    Guid PaidByUserId,
    // lista de quem vai dividir o gasto e quanto cada um deve
    // dessa forma suportamos divisão igual E divisão personalizada
    IEnumerable<ExpenseSplitRequest> Splits
);