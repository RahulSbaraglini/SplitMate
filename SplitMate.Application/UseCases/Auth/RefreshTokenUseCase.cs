using SplitMate.Application.DTOs;
using SplitMate.Application.Interfaces;

namespace SplitMate.Application.UseCases.Auth;

public class RefreshTokenUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenUseCase(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> ExecuteAsync(RefreshTokenRequest request)
    {
        // buscamos o usuário que possui esse refresh token salvo
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);

        // se não encontrar o usuário, ou o token salvo já expirou, rejeitamos
        if (user is null || user.RefreshTokenExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token inválido ou expirado.");

        // geramos um novo par de tokens — isso é chamado de "rotação de refresh token"
        // invalidar o token antigo e gerar um novo a cada uso impede que um token roubado
        // continue sendo reutilizado indefinidamente
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        await _userRepository.UpdateAsync(user);

        return new AuthResponse(user.Id, user.Name, user.Email, newAccessToken, newRefreshToken);
    }
}