using Microsoft.EntityFrameworkCore;
using SplitMate.Application.Interfaces;
using SplitMate.Domain.Entities;
using SplitMate.Infrastructure.Persistence;

namespace SplitMate.Infrastructure.Repositories;

// essa classe implementa o contrato definido pela Application
// toda a lógica de banco de dados fica aqui — os use cases não sabem que o EF Core existe
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    // recebemos o AppDbContext via injeção de dependência
    // Clean Code — a classe não cria suas próprias dependências, ela as recebe
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
        => await _context.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email)
        // AsNoTracking melhora a performance em consultas de leitura
        // o EF Core não precisa monitorar mudanças no objeto se só vamos ler
        => await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> ExistsByEmailAsync(string email)
        => await _context.Users.AnyAsync(u => u.Email == email);

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<User>> GetByIdsAsync(IEnumerable<Guid> ids)
    => await _context.Users
        .Where(u => ids.Contains(u.Id))
        .AsNoTracking()
        .ToListAsync();

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        => await _context.Users
        .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
}