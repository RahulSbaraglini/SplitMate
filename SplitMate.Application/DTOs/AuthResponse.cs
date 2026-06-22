namespace SplitMate.Application.DTOs;

// DTO de saída — é exatamente o que a API devolve após login ou registro bem sucedido
// note que não devolvemos a senha nem o PasswordHash — nunca expor dados sensíveis
public record AuthResponse(
    Guid UserId,
    string Name,
    string Email,
    string AccessToken,
    string RefreshToken
);