using System.Linq.Expressions;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Backend.Infrastructure.Repositories;

public class RefreshTokenRepository(LiftLogDatabase database) : IRefreshTokenRepository
{
    private readonly LiftLogDatabase _database =
        database ?? throw new ArgumentNullException(nameof(database));

    public async Task<bool> HasAsync(
        Expression<Func<RefreshToken, bool>> predicate,
        CancellationToken cancellationToken = default
    ) => await _database.Set<RefreshToken>().AsNoTracking().AnyAsync(predicate, cancellationToken);

    public async Task<RefreshToken?> FindAsync(
        Expression<Func<RefreshToken, bool>> predicate,
        CancellationToken cancellationToken = default
    ) =>
        await _database
            .Set<RefreshToken>()
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);

    public Task<List<RefreshToken>> FindMultipleAsync(
        Expression<Func<RefreshToken, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public Task<List<RefreshToken>> FindBySearchAsync(
        string searchText,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<RefreshToken> UpdateAsync(
        RefreshToken entity,
        CancellationToken cancellationToken = default
    ) => await Task.FromResult(_database.RefreshTokens.Update(entity).Entity);

    public async Task<RefreshToken> CreateAsync(
        RefreshToken entity,
        CancellationToken cancellationToken = default
    )
    {
        var entry = await _database.RefreshTokens.AddAsync(entity, cancellationToken);
        return await Task.FromResult(entry.Entity);
    }

    public async Task<bool> KillAsync(Guid id, CancellationToken cancellationToken = default) =>
        await FindAsync(x => x.Id == id, cancellationToken) is { } refreshToken
        && (_database.RefreshTokens.Remove(refreshToken) is var _ || true);
}
