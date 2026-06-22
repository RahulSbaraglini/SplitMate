using Microsoft.EntityFrameworkCore;
using SplitMate.Application.Interfaces;
using SplitMate.Domain.Entities;
using SplitMate.Infrastructure.Persistence;

namespace SplitMate.Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly AppDbContext _context;

    public GroupRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Group?> GetByIdAsync(Guid id)
        => await _context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == id);

    public async Task<Group?> GetByInviteCodeAsync(string inviteCode)
        => await _context.Groups
            .FirstOrDefaultAsync(g => g.InviteCode == inviteCode);

    public async Task<IReadOnlyList<Group>> GetByUserIdAsync(Guid userId)
        => await _context.Groups
            .Include(g => g.Members)
            .Where(g => g.Members.Any(m => m.UserId == userId))
            .AsNoTracking()
            .ToListAsync();

    public async Task AddAsync(Group group)
    {
        await _context.Groups.AddAsync(group);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsMemberAsync(Guid groupId, Guid userId)
        => await _context.GroupMembers
            .AnyAsync(m => m.GroupId == groupId && m.UserId == userId);

    public async Task AddMemberAsync(GroupMember member)
    {
        await _context.GroupMembers.AddAsync(member);
        await _context.SaveChangesAsync();
    }
}