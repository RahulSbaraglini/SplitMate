using Microsoft.EntityFrameworkCore;
using SplitMate.Application.Interfaces;
using SplitMate.Domain.Entities;
using SplitMate.Infrastructure.Persistence;

namespace SplitMate.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _context;

    public ExpenseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Expense>> GetByGroupIdAsync(Guid groupId)
        => await _context.Expenses
            // Include carrega os dados relacionados junto com a consulta
            // sem isso os Splits viriam vazios — seria necessário uma segunda consulta
            .Include(e => e.Splits)
            .Include(e => e.PaidBy)
            .Where(e => e.GroupId == groupId)
            .AsNoTracking()
            .ToListAsync();

    public async Task AddAsync(Expense expense)
    {
        await _context.Expenses.AddAsync(expense);
        await _context.SaveChangesAsync();
    }
}