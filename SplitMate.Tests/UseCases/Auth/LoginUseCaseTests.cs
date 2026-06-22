using Moq;
using SplitMate.Application.DTOs;
using SplitMate.Application.Interfaces;
using SplitMate.Application.UseCases.Auth;
using SplitMate.Domain.Entities;

namespace SplitMate.Tests.UseCases.Auth;

public class LoginUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly LoginUseCase _useCase;

    public LoginUseCaseTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _useCase = new LoginUseCase(_userRepositoryMock.Object, _jwtTokenServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ShouldThrowUnauthorized()
    {
        // Arrange — simulamos que nenhum usuário foi encontrado com esse email
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var request = new LoginRequest("naoexiste@email.com", "123456");

        // Act + Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_WhenPasswordIsWrong_ShouldThrowUnauthorized()
    {
        // Arrange — criamos um usuário real com uma senha conhecida
        // usamos o BCrypt de verdade aqui porque é exatamente isso que o LoginUseCase compara
        var realPasswordHash = BCrypt.Net.BCrypt.HashPassword("senhacerta");
        var user = User.Create("João Silva", "joao@email.com", realPasswordHash);

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        // tentamos logar com uma senha diferente da que foi cadastrada
        var request = new LoginRequest("joao@email.com", "senhaerrada");

        // Act + Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_WhenCredentialsAreCorrect_ShouldReturnTokens()
    {
        // Arrange
        var realPasswordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        var user = User.Create("João Silva", "joao@email.com", realPasswordHash);

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        _jwtTokenServiceMock
            .Setup(j => j.GenerateAccessToken(It.IsAny<User>()))
            .Returns("fake-access-token");

        _jwtTokenServiceMock
            .Setup(j => j.GenerateRefreshToken())
            .Returns("fake-refresh-token");

        var request = new LoginRequest("joao@email.com", "123456");

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.Equal("João Silva", result.Name);
        Assert.Equal("fake-access-token", result.AccessToken);

        // confirmamos que o refresh token foi salvo no usuário após o login
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }
}