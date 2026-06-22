using Moq;
using SplitMate.Application.DTOs;
using SplitMate.Application.Interfaces;
using SplitMate.Application.UseCases.Auth;
using SplitMate.Domain.Entities;

namespace SplitMate.Tests.UseCases.Auth;

public class RegisterUseCaseTests
{
    // Mock<T> cria uma versão falsa da interface — o teste não toca no banco real
    // isso é o que torna o teste rápido, isolado e repetível
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly RegisterUseCase _useCase;

    public RegisterUseCaseTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();

        // injetamos os mocks no lugar das implementações reais
        // o use case nem sabe que está recebendo um mock — ele só conhece a interface
        _useCase = new RegisterUseCase(_userRepositoryMock.Object, _jwtTokenServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailAlreadyExists_ShouldThrowException()
    {
        // Arrange — configuramos o mock para simular que o email já existe
        _userRepositoryMock
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var request = new RegisterRequest("João Silva", "joao@email.com", "123456");

        // Act + Assert — verificamos que a exceção correta é lançada
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(request));

        // garantimos que, como o email já existia, o usuário NUNCA foi salvo
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailIsNew_ShouldCreateUserAndReturnTokens()
    {
        // Arrange — simulamos que o email está disponível
        _userRepositoryMock
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _jwtTokenServiceMock
            .Setup(j => j.GenerateAccessToken(It.IsAny<User>()))
            .Returns("fake-access-token");

        _jwtTokenServiceMock
            .Setup(j => j.GenerateRefreshToken())
            .Returns("fake-refresh-token");

        var request = new RegisterRequest("João Silva", "joao@email.com", "123456");

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert — confirmamos que o retorno está correto
        Assert.Equal("João Silva", result.Name);
        Assert.Equal("joao@email.com", result.Email);
        Assert.Equal("fake-access-token", result.AccessToken);
        Assert.Equal("fake-refresh-token", result.RefreshToken);

        // confirmamos que o usuário foi de fato salvo no repositório
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }
}