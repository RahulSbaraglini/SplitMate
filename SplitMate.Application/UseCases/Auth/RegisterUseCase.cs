using SplitMate.Application.DTOs;
using SplitMate.Application.Interfaces;
using SplitMate.Domain.Entities;

namespace SplitMate.Application.UseCases.Auth;

// responsabilidade única — essa classe só sabe registrar um novo usuário
// ela não sabe nada de HTTP, banco de dados ou JWT diretamente
// apenas orquestra as dependências que recebe via injeção
public class RegisterUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService  _jwtTokenService;

    public RegisterUseCase(IUserRepository userRepository, IJwtTokenService  jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> ExecuteAsync(RegisterRequest request)
    {
        // regra de negócio — email já cadastrado não pode se registrar novamente
        var emailAlreadyExists = await _userRepository.ExistsByEmailAsync(request.Email);
        if (emailAlreadyExists)
            throw new InvalidOperationException("Este e-mail já está cadastrado.");

        // BCrypt gera um hash seguro da senha — nunca salvamos a senha em texto puro
        // o número 12 é o "work factor" — quanto maior, mais seguro e mais lento
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

        // usamos o factory method Create que criamos no Domain
        // ele garante que o usuário nasce num estado válido
        var user = User.Create(request.Name, request.Email, passwordHash);

        // geramos os tokens antes de salvar para garantir que tudo está ok
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // salvamos o refresh token no usuário para validar no futuro
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

        await _userRepository.AddAsync(user);

        // devolvemos um DTO limpo — sem expor detalhes internos do domínio
        return new AuthResponse(user.Id, user.Name, user.Email, accessToken, refreshToken);
    }
}