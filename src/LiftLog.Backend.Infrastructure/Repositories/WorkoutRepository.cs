using System.Linq.Expressions;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Backend.Infrastructure.Repositories;

public class WorkoutRepository(LiftLogDatabase database) : IWorkoutRepository
{
    private readonly LiftLogDatabase _database =
        database ?? throw new ArgumentNullException(nameof(database));

    public async Task<bool> HasAsync(
        Expression<Func<Workout, bool>> predicate,
        CancellationToken cancellationToken = default
    ) => await _database.Workouts.AsNoTracking().AnyAsync(predicate, cancellationToken);

    public async Task<Workout?> FindAsync(
        Expression<Func<Workout, bool>> predicate,
        CancellationToken cancellationToken = default
    ) => await _database.Workouts.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<List<Workout>> FindMultipleAsync(
        Expression<Func<Workout, bool>> predicate,
        CancellationToken cancellationToken = default
    ) => await _database.Workouts.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task<List<Workout>> FindBySearchAsync(
        string searchText,
        CancellationToken cancellationToken = default
    ) =>
        await _database
            .Workouts.AsNoTracking()
            .Where(x =>
                EF.Functions.ILike(
                    EF.Functions.Unaccent(x.Name),
                    EF.Functions.Unaccent($"%{searchText}%")
                )
            )
            .ToListAsync(cancellationToken);

    public async Task<Workout> UpdateAsync(
        Workout entity,
        CancellationToken cancellationToken = default
    ) => await Task.FromResult(_database.Workouts.Update(entity).Entity);

    public async Task<Workout> CreateAsync(
        Workout entity,
        CancellationToken cancellationToken = default
    )
    {
        var entry = await _database.Workouts.AddAsync(entity, cancellationToken);
        return await Task.FromResult(entry.Entity);
    }

    public async Task<bool> KillAsync(Guid id, CancellationToken cancellationToken = default) =>
        await FindAsync(x => x.Id == id, cancellationToken) is { } workout
        && (_database.Workouts.Remove(workout) is var _ || true);
}
