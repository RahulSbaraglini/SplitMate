using SplitMate.Domain.Entities;

namespace SplitMate.Application.Interfaces;

// essa interface é o contrato que a Application conhece
// ela não sabe se os dados vêm do SQL Server, SQLite ou até de um arquivo de texto
// isso é Clean Architecture na prática — dependa de abstrações, não de implementações
public interface IUserRepository
{
    // cada método tem um nome que descreve exatamente o que faz — Clean Code
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<IReadOnlyList<User>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
}