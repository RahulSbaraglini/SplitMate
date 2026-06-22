using SplitMate.Domain.Entities;

namespace SplitMate.Application.Interfaces;

public interface IExpenseRepository
{
    Task<IReadOnlyList<Expense>> GetByGroupIdAsync(Guid groupId);
    Task AddAsync(Expense expense);
}