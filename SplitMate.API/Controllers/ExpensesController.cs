using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitMate.Application.DTOs;
using SplitMate.Application.UseCases.Expenses;

namespace SplitMate.API.Controllers;

[ApiController]
[Route("api/groups/{groupId}/expenses")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly AddExpenseUseCase _addExpenseUseCase;
    private readonly GetGroupExpensesUseCase _getGroupExpensesUseCase;
    private readonly GetGroupDebtsUseCase _getGroupDebtsUseCase;

    public ExpensesController(
        AddExpenseUseCase addExpenseUseCase,
        GetGroupExpensesUseCase getGroupExpensesUseCase,
        GetGroupDebtsUseCase getGroupDebtsUseCase)
    {
        _addExpenseUseCase = addExpenseUseCase;
        _getGroupExpensesUseCase = getGroupExpensesUseCase;
        _getGroupDebtsUseCase = getGroupDebtsUseCase;
    }

    // o groupId vem da rota — api/groups/{groupId}/expenses
    // isso é REST semântico — as despesas pertencem a um grupo
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> AddExpense(Guid groupId, AddExpenseRequest request)
    {
        try
        {
            var response = await _addExpenseUseCase.ExecuteAsync(groupId, request, GetUserId());
            return Created(string.Empty, response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetExpenses(Guid groupId)
    {
        try
        {
            var response = await _getGroupExpensesUseCase.ExecuteAsync(groupId, GetUserId());
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    // rota separada para o cálculo de dívidas — GET api/groups/{groupId}/expenses/debts
    [HttpGet("debts")]
    public async Task<IActionResult> GetDebts(Guid groupId)
    {
        try
        {
            var response = await _getGroupDebtsUseCase.ExecuteAsync(groupId, GetUserId());
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}