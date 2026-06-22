using SplitMate.Application.DTOs;
using SplitMate.Application.Interfaces;

namespace SplitMate.Application.UseCases.Expenses;

public class GetGroupExpensesUseCase
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GetGroupExpensesUseCase(
        IExpenseRepository expenseRepository,
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _expenseRepository = expenseRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<ExpenseResponse>> ExecuteAsync(Guid groupId, Guid currentUserId)
    {
        var isMember = await _groupRepository.IsMemberAsync(groupId, currentUserId);
        if (!isMember)
            throw new UnauthorizedAccessException("Você não é membro deste grupo.");

        var expenses = await _expenseRepository.GetByGroupIdAsync(groupId);

        // buscamos os nomes dos usuários de uma vez só — evitamos N+1 queries
        // N+1 query é um problema comum onde você faz uma query pra cada item da lista
        // aqui buscamos todos os usuários relevantes em uma única consulta
        var userIds = expenses.Select(e => e.PaidByUserId).Distinct();
        var users = await _userRepository.GetByIdsAsync(userIds);
        var userDict = users.ToDictionary(u => u.Id, u => u.Name);

        return expenses.Select(e => new ExpenseResponse(
            e.Id,
            e.Description,
            e.Amount,
            e.PaidByUserId,
            userDict.GetValueOrDefault(e.PaidByUserId, "Desconhecido"),
            e.Date,
            e.Splits.Select(s => new ExpenseSplitResponse(s.UserId, s.Amount))
        )).ToList();
    }
}