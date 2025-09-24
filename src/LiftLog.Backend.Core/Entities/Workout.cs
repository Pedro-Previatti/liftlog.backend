using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Validators;

namespace LiftLog.Backend.Core.Entities;

public class Workout : BaseEntity
{
    public required List<Guid> WorkoutExerciseIds { get; init; }
    public required List<Guid> CreatedForUserIds { get; init; }

    public required string Name { get; set; }

    protected Workout() { }

    [SetsRequiredMembers]
    private Workout(
        List<Guid> workoutExerciseIds,
        List<Guid> createdForUserIds,
        Guid createdBy,
        string name
    )
    {
        CreatedBy = createdBy;
        UpdatedBy = createdBy;

        CreatedForUserIds = createdForUserIds;
        WorkoutExerciseIds = workoutExerciseIds;
        Name = name;

        Validate(this, new WorkoutValidator());
    }

    public static Workout Create(
        List<Guid> workoutExerciseIds,
        List<Guid> createdForUserIds,
        Guid createdBy,
        string name
    ) => new(workoutExerciseIds, createdForUserIds, createdBy, name);

    public void Update(
        Guid updatedBy,
        List<Guid>? workoutExerciseIds = null,
        List<Guid>? createdForUserIds = null,
        string? name = null
    )
    {
        UpdatedBy = updatedBy;
        UpdatedAtUtc = DateTime.UtcNow;

        if (workoutExerciseIds is not null && workoutExerciseIds.Count > 0)
            WorkoutExerciseIds.AddRange(
                workoutExerciseIds.Where(exerciseId => !WorkoutExerciseIds.Contains(exerciseId))
            );

        if (createdForUserIds is not null && createdForUserIds.Count > 0)
            CreatedForUserIds.AddRange(
                createdForUserIds.Where(userId => !WorkoutExerciseIds.Contains(userId))
            );

        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        Validate(this, new WorkoutValidator());
    }

    public void RemoveUserFromWorkout(Guid updatedBy, Guid userId)
    {
        UpdatedBy = updatedBy;
        UpdatedAtUtc = DateTime.UtcNow;

        WorkoutExerciseIds.Remove(userId);

        Validate(this, new WorkoutValidator());
    }
}
