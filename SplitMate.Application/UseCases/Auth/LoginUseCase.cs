using SplitMate.Application.DTOs;
using SplitMate.Application.Interfaces;

namespace SplitMate.Application.UseCases.Auth;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService  _jwtTokenService;

    public LoginUseCase(IUserRepository userRepository, IJwtTokenService  jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> ExecuteAsync(LoginRequest request)
    {
        // buscamos o usuário pelo email
        var user = await _userRepository.GetByEmailAsync(request.Email);

        // usamos a mesma mensagem de erro para email e senha inválidos
        // isso é uma boa prática de segurança — não revelamos se o email existe ou não
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await _userRepository.UpdateAsync(user);

        return new AuthResponse(user.Id, user.Name, user.Email, accessToken, refreshToken);
    }
}