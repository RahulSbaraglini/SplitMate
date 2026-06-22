using SplitMate.Domain.Entities;

namespace SplitMate.Application.Interfaces;

// a Application só conhece o contrato — não sabe que existe um JwtTokenService
// assim como fez com os repositórios, a Application define a interface
// e a Infrastructure implementa
public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}