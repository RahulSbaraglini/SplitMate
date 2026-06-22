using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SplitMate.Application.Settings;
using SplitMate.Domain.Entities;
using SplitMate.Application.Interfaces;

namespace SplitMate.Infrastructure.Services;

// responsabilidade única: essa classe só sabe gerar e validar tokens JWT
// ela não sabe nada de banco de dados, controllers ou regras de negócio
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    // recebemos as configurações via injeção de dependência
    // isso é Clean Code — a classe não busca as configurações, elas são fornecidas
    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateAccessToken(User user)
    {
        // claims são as informações que ficam dentro do token
        // colocamos só o necessário — id e email do usuário
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // a chave secreta é usada para assinar o token
        // só quem tem essa chave consegue validar se o token é legítimo
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        // o refresh token é um valor aleatório e opaco
        // ele não carrega informações como o JWT — só serve pra solicitar um novo access token
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}