using SplitMate.Application.DTOs;
using SplitMate.Application.Interfaces;
using SplitMate.Domain.Services;

namespace SplitMate.Application.UseCases.Expenses;

public class GetGroupDebtsUseCase
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GetGroupDebtsUseCase(
        IExpenseRepository expenseRepository,
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _expenseRepository = expenseRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<DebtResponse>> ExecuteAsync(Guid groupId, Guid currentUserId)
    {
        var isMember = await _groupRepository.IsMemberAsync(groupId, currentUserId);
        if (!isMember)
            throw new UnauthorizedAccessException("Você não é membro deste grupo.");

        var expenses = await _expenseRepository.GetByGroupIdAsync(groupId);

        // convertemos as despesas para o formato que o DebtCalculator espera
        // aqui o Domain Service entra em ação — ele não sabe nada de banco ou HTTP
        var expenseData = expenses.Select(e => (
            e.PaidByUserId,
            e.Amount,
            e.Splits.Select(s => (s.UserId, s.Amount))
        ));

        // o DebtCalculator retorna o número mínimo de transferências para zerar as dívidas
        var transactions = DebtCalculator.Calculate(expenseData);

        if (!transactions.Any())
            return new List<DebtResponse>();

        // buscamos os nomes de todos os usuários envolvidos nas transações
        var userIds = transactions
            .SelectMany(t => new[] { t.FromUserId, t.ToUserId })
            .Distinct();
        var users = await _userRepository.GetByIdsAsync(userIds);
        var userDict = users.ToDictionary(u => u.Id, u => u.Name);

        return transactions.Select(t => new DebtResponse(
            t.FromUserId,
            userDict.GetValueOrDefault(t.FromUserId, "Desconhecido"),
            t.ToUserId,
            userDict.GetValueOrDefault(t.ToUserId, "Desconhecido"),
            t.Amount
        )).ToList();
    }
}