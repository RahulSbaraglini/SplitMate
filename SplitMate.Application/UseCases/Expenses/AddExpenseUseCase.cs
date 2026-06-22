using SplitMate.Application.DTOs;
using SplitMate.Application.Interfaces;
using SplitMate.Domain.Entities;

namespace SplitMate.Application.UseCases.Expenses;

public class AddExpenseUseCase
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IGroupRepository _groupRepository;

    public AddExpenseUseCase(
        IExpenseRepository expenseRepository,
        IGroupRepository groupRepository)
    {
        _expenseRepository = expenseRepository;
        _groupRepository = groupRepository;
    }

    public async Task<ExpenseResponse> ExecuteAsync(
        Guid groupId, AddExpenseRequest request, Guid currentUserId)
    {
        // verificamos se o usuário que está adicionando o gasto é membro do grupo
        // isso é a autorização por grupo — Clean Architecture protegendo o negócio
        var isMember = await _groupRepository.IsMemberAsync(groupId, currentUserId);
        if (!isMember)
            throw new UnauthorizedAccessException("Você não é membro deste grupo.");

        // usamos o factory method Create do Domain — ele valida o valor automaticamente
        var expense = Expense.Create(
            request.Description,
            request.Amount,
            request.PaidByUserId,
            groupId
        );

        // criamos os splits — cada um representa a parte de um membro no gasto
        foreach (var split in request.Splits)
        {
            var expenseSplit = ExpenseSplit.Create(expense.Id, split.UserId, split.Amount);
            expense.Splits.Add(expenseSplit);
        }

        await _expenseRepository.AddAsync(expense);

        return new ExpenseResponse(
            expense.Id,
            expense.Description,
            expense.Amount,
            expense.PaidByUserId,
            string.Empty, // nome será preenchido numa consulta separada se necessário
            expense.Date,
            expense.Splits.Select(s => new ExpenseSplitResponse(s.UserId, s.Amount))
        );
    }
}