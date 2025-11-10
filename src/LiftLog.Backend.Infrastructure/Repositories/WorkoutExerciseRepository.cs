using System.Linq.Expressions;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Backend.Infrastructure.Repositories;

public class WorkoutExerciseRepository(LiftLogDatabase database) : IWorkoutExerciseRepository
{
    private readonly LiftLogDatabase _database =
        database ?? throw new ArgumentNullException(nameof(database));

    public async Task<bool> HasAsync(
        Expression<Func<WorkoutExercise, bool>> predicate,
        CancellationToken cancellationToken = default
    ) => await _database.WorkoutExercises.AsNoTracking().AnyAsync(predicate, cancellationToken);

    public async Task<WorkoutExercise?> FindAsync(
        Expression<Func<WorkoutExercise, bool>> predicate,
        CancellationToken cancellationToken = default
    ) =>
        await _database
            .WorkoutExercises.AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<List<WorkoutExercise>> FindMultipleAsync(
        Expression<Func<WorkoutExercise, bool>> predicate,
        CancellationToken cancellationToken = default
    ) =>
        await _database
            .WorkoutExercises.AsNoTracking()
            .Where(predicate)
            .ToListAsync(cancellationToken);

    public async Task<List<WorkoutExercise>> FindBySearchAsync(
        string searchText,
        CancellationToken cancellationToken = default
    ) =>
        await _database
            .WorkoutExercises.AsNoTracking()
            .Where(x =>
                EF.Functions.ILike(
                    EF.Functions.Unaccent(x.ExerciseName),
                    EF.Functions.Unaccent($"%{searchText}%")
                )
            )
            .ToListAsync(cancellationToken);

    public async Task<WorkoutExercise> UpdateAsync(
        WorkoutExercise entity,
        CancellationToken cancellationToken = default
    ) => await Task.FromResult(_database.WorkoutExercises.Update(entity).Entity);

    public async Task<WorkoutExercise> CreateAsync(
        WorkoutExercise entity,
        CancellationToken cancellationToken = default
    )
    {
        var entry = await _database.WorkoutExercises.AddAsync(entity, cancellationToken);
        return await Task.FromResult(entry.Entity);
    }

    public async Task<bool> KillAsync(Guid id, CancellationToken cancellationToken = default) =>
        await FindAsync(x => x.Id == id, cancellationToken) is { } workoutExercise
        && (_database.WorkoutExercises.Remove(workoutExercise) is var _ || true);
}
