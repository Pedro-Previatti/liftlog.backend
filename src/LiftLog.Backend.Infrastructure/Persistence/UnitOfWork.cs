using LiftLog.Backend.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace LiftLog.Backend.Infrastructure.Persistence;

public class UnitOfWork(LiftLogDatabase database) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    private readonly LiftLogDatabase _database =
        database ?? throw new ArgumentNullException(nameof(database));

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        _transaction = await _database.Database.BeginTransactionAsync(cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _database.SaveChangesAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _database.SaveChangesAsync(cancellationToken);

        if (_transaction is not null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();

            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();

            _transaction = null;
        }
    }

    public void Dispose() => _transaction?.Dispose();
}
