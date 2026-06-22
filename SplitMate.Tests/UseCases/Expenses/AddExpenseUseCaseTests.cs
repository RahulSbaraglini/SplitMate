using Moq;
using SplitMate.Application.DTOs;
using SplitMate.Application.Interfaces;
using SplitMate.Application.UseCases.Expenses;
using SplitMate.Domain.Entities;

namespace SplitMate.Tests.UseCases.Expenses;

public class AddExpenseUseCaseTests
{
    private readonly Mock<IExpenseRepository> _expenseRepositoryMock;
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly AddExpenseUseCase _useCase;

    public AddExpenseUseCaseTests()
    {
        _expenseRepositoryMock = new Mock<IExpenseRepository>();
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _useCase = new AddExpenseUseCase(_expenseRepositoryMock.Object, _groupRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserIsNotGroupMember_ShouldThrowUnauthorized()
    {
        // Arrange — simulamos que o usuário NÃO pertence ao grupo
        _groupRepositoryMock
            .Setup(r => r.IsMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        var groupId = Guid.NewGuid();
        var outsiderUserId = Guid.NewGuid(); // alguém de fora do grupo
        var paidByUserId = Guid.NewGuid();

        var request = new AddExpenseRequest(
            "Jantar",
            100m,
            paidByUserId,
            new[] { new ExpenseSplitRequest(paidByUserId, 100m) }
        );

        // Act + Assert — a tentativa deve ser bloqueada antes de qualquer gasto ser criado
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _useCase.ExecuteAsync(groupId, request, outsiderUserId));

        // garantimos que NENHUM gasto foi salvo, já que o usuário não tinha permissão
        _expenseRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Expense>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserIsGroupMember_ShouldCreateExpenseWithSplits()
    {
        // Arrange — agora o usuário É membro do grupo
        _groupRepositoryMock
            .Setup(r => r.IsMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        var groupId = Guid.NewGuid();
        var memberUserId = Guid.NewGuid();
        var paidByUserId = Guid.NewGuid();
        var otherMemberId = Guid.NewGuid();

        var request = new AddExpenseRequest(
            "Jantar",
            100m,
            paidByUserId,
            new[]
            {
                new ExpenseSplitRequest(paidByUserId, 50m),
                new ExpenseSplitRequest(otherMemberId, 50m)
            }
        );

        // Act
        var result = await _useCase.ExecuteAsync(groupId, request, memberUserId);

        // Assert
        Assert.Equal("Jantar", result.Description);
        Assert.Equal(100m, result.Amount);
        Assert.Equal(2, result.Splits.Count());

        // confirmamos que o gasto foi realmente persistido
        _expenseRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Expense>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAmountIsNegative_ShouldThrowArgumentException()
    {
        // Arrange — o usuário é membro válido, mas o valor do gasto é inválido
        // esse teste confirma que a regra de negócio do Domain (Expense.Create) é respeitada
        _groupRepositoryMock
            .Setup(r => r.IsMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var request = new AddExpenseRequest(
            "Jantar",
            -50m, // valor inválido
            userId,
            new[] { new ExpenseSplitRequest(userId, -50m) }
        );

        // Act + Assert — a exceção vem do Domain (Expense.Create), não do use case
        await Assert.ThrowsAsync<ArgumentException>(
            () => _useCase.ExecuteAsync(groupId, request, userId));
    }
}