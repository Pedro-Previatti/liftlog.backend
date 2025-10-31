using System.Linq.Expressions;

namespace LiftLog.Backend.Core.Interfaces.Repositories;

public interface IRepository<T>
{
    Task<bool> HasAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    Task<T?> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    Task<List<T>> FindMultipleAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    Task<List<T>> FindBySearchAsync(
        string searchText,
        CancellationToken cancellationToken = default
    );

    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);

    Task<bool> KillAsync(Guid id, CancellationToken cancellationToken = default);
}
