using System.Linq.Expressions;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Backend.Infrastructure.Repositories;

public class MuscleGroupRepository(LiftLogDatabase database) : IMuscleGroupRepository
{
    private readonly LiftLogDatabase _database =
        database ?? throw new ArgumentNullException(nameof(database));

    public async Task<bool> HasAsync(
        Expression<Func<MuscleGroup, bool>> predicate,
        CancellationToken cancellationToken = default
    ) => await _database.Set<MuscleGroup>().AsNoTracking().AnyAsync(predicate, cancellationToken);

    public async Task<MuscleGroup?> FindAsync(
        Expression<Func<MuscleGroup, bool>> predicate,
        CancellationToken cancellationToken = default
    ) =>
        await _database
            .Set<MuscleGroup>()
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<List<MuscleGroup>> FindMultipleAsync(
        Expression<Func<MuscleGroup, bool>> predicate,
        CancellationToken cancellationToken = default
    ) =>
        await _database
            .Set<MuscleGroup>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(cancellationToken);

    public async Task<List<MuscleGroup>> FindBySearchAsync(
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

        var query = _database.MuscleGroups.AsQueryable();

        query = query.Where(x =>
            terms.Any(term =>
                EF.Functions.ILike(
                    EF.Functions.Unaccent(x.Name),
                    EF.Functions.Unaccent($"%{term}%")
                )
            )
        );

        return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public Task<MuscleGroup> UpdateAsync(
        MuscleGroup entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public Task<MuscleGroup> CreateAsync(
        MuscleGroup entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public Task<bool> KillAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
