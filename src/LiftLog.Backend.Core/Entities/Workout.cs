using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Validators;

namespace LiftLog.Backend.Core.Entities;

public class Workout : BaseEntity
{
    public required List<Guid> WorkoutExerciseIds { get; init; }
    public required List<Guid> CreatedForUserIds { get; init; }

    public required string Name { get; set; }

    public required DayOfWeek DayOfWeek { get; set; }

    protected Workout() { }

    [SetsRequiredMembers]
    private Workout(
        List<Guid> workoutExerciseIds,
        List<Guid> createdForUserIds,
        Guid createdBy,
        string name,
        DayOfWeek dayOfWeek
    )
    {
        CreatedBy = createdBy;
        UpdatedBy = createdBy;

        CreatedForUserIds = createdForUserIds;
        WorkoutExerciseIds = workoutExerciseIds;
        Name = name;
        DayOfWeek = dayOfWeek;

        Validate(this, new WorkoutValidator());
    }

    public static Workout Create(
        List<Guid> workoutExerciseIds,
        List<Guid> createdForUserIds,
        Guid createdBy,
        string name,
        DayOfWeek dayOfWeek
    ) => new(workoutExerciseIds, createdForUserIds, createdBy, name, dayOfWeek);

    public void Update(
        Guid updatedBy,
        List<Guid>? workoutExerciseIds = null,
        List<Guid>? createdForUserIds = null,
        string? name = null,
        DayOfWeek? dayOfWeek = null
    )
    {
        if (updatedBy.Equals(Guid.Empty))
            throw new ArgumentException("Workout UpdatedBy cannot be empty.", nameof(updatedBy));

        UpdatedBy = updatedBy;
        UpdatedAtUtc = DateTime.UtcNow;

        if (workoutExerciseIds is not null && workoutExerciseIds.Count > 0)
            WorkoutExerciseIds.AddRange(
                workoutExerciseIds.Where(exerciseId => !WorkoutExerciseIds.Contains(exerciseId))
            );

        if (createdForUserIds is not null && createdForUserIds.Count > 0)
            CreatedForUserIds.AddRange(
                createdForUserIds.Where(userId => !CreatedForUserIds.Contains(userId))
            );

        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        if (dayOfWeek.HasValue)
            DayOfWeek = dayOfWeek.Value;

        Validate(this, new WorkoutValidator());
    }

    public void RemoveExerciseFromWorkout(Guid updatedBy, Guid exerciseId)
    {
        if (updatedBy.Equals(Guid.Empty))
            throw new ArgumentException("Workout UpdatedBy cannot be empty.", nameof(updatedBy));

        UpdatedBy = updatedBy;
        UpdatedAtUtc = DateTime.UtcNow;

        WorkoutExerciseIds.Remove(exerciseId);

        Validate(this, new WorkoutValidator());
    }

    public void RemoveUserFromWorkout(Guid updatedBy, Guid userId)
    {
        if (updatedBy.Equals(Guid.Empty))
            throw new ArgumentException("Workout UpdatedBy cannot be empty.", nameof(updatedBy));

        UpdatedBy = updatedBy;
        UpdatedAtUtc = DateTime.UtcNow;

        CreatedForUserIds.Remove(userId);

        Validate(this, new WorkoutValidator());
    }
}
