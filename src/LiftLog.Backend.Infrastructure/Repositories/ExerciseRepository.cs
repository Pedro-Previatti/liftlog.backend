using System.Linq.Expressions;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Backend.Infrastructure.Repositories;

public class ExerciseRepository(LiftLogDatabase database) : IExerciseRepository
{
    private readonly LiftLogDatabase _database =
        database ?? throw new ArgumentNullException(nameof(database));

    public async Task<bool> HasAsync(
        Expression<Func<Exercise, bool>> predicate,
        CancellationToken cancellationToken = default
    ) => await _database.Set<Exercise>().AsNoTracking().AnyAsync(predicate, cancellationToken);

    public async Task<Exercise?> FindAsync(
        Expression<Func<Exercise, bool>> predicate,
        CancellationToken cancellationToken = default
    ) =>
        await _database
            .Set<Exercise>()
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<List<Exercise>> FindMultipleAsync(
        Expression<Func<Exercise, bool>> predicate,
        CancellationToken cancellationToken = default
    ) =>
        await _database
            .Set<Exercise>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(cancellationToken);

    public async Task<List<Exercise>> FindBySearchAsync(
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

        var query = _database.Exercises.AsQueryable();

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

    public Task<Exercise> UpdateAsync(
        Exercise entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public Task<Exercise> CreateAsync(
        Exercise entity,
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
