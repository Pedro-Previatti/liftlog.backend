using System.Linq.Expressions;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Backend.Infrastructure.Repositories;

public class UserRepository(LiftLogDatabase database) : IUserRepository
{
    private readonly LiftLogDatabase _database =
        database ?? throw new ArgumentNullException(nameof(database));

    public async Task<bool> HasAsync(
        Expression<Func<User, bool>> predicate,
        CancellationToken cancellationToken = default
    ) => await _database.Users.AsNoTracking().AnyAsync(predicate, cancellationToken);

    public async Task<User?> FindAsync(
        Expression<Func<User, bool>> predicate,
        CancellationToken cancellationToken = default
    ) => await _database.Users.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<List<User>> FindMultipleAsync(
        Expression<Func<User, bool>> predicate,
        CancellationToken cancellationToken = default
    ) => await _database.Users.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task<List<User>> FindBySearchAsync(
        string searchText,
        CancellationToken cancellationToken = default
    )
    {
        var terms = searchText
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .ToList();

        var query = _database.Users.AsQueryable();

        query = query.Where(x =>
            !string.IsNullOrWhiteSpace(x.SearchText)
            && terms.Any(term =>
                EF.Functions.ILike(
                    EF.Functions.Unaccent(x.SearchText),
                    EF.Functions.Unaccent($"%{term}%")
                )
            )
        );

        return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default) =>
        await Task.FromResult(_database.Users.Update(user).Entity);

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        var entry = await _database.Users.AddAsync(user, cancellationToken);
        return await Task.FromResult(entry.Entity);
    }

    public async Task<bool> KillAsync(Guid id, CancellationToken cancellationToken = default) =>
        await FindAsync(x => x.Id == id, cancellationToken) is { } user
        && (_database.Users.Remove(user) is var _ || true);
}
