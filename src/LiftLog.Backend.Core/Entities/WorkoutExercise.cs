using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Enums;
using LiftLog.Backend.Core.Validators;

namespace LiftLog.Backend.Core.Entities;

public class WorkoutExercise : BaseEntity
{
    public required Guid ExerciseId { get; set; }

    public required string ExerciseName { get; set; }

    public required WeightUnit WeightUnit { get; set; }

    public required int Sets { get; set; }
    public required int Reps { get; set; }

    public required float Weight { get; set; }

    protected WorkoutExercise() { }

    [SetsRequiredMembers]
    private WorkoutExercise(
        Guid createdBy,
        Guid exerciseId,
        string exerciseName,
        int sets,
        int reps,
        float weight,
        WeightUnit weightUnit
    )
    {
        CreatedBy = createdBy;
        UpdatedBy = createdBy;

        ExerciseId = exerciseId;
        ExerciseName = exerciseName;
        Sets = sets;
        Reps = reps;
        Weight = weight;
        WeightUnit = weightUnit;

        Validate(this, new WorkoutExerciseValidator());
    }

    public static WorkoutExercise Create(
        Guid createdBy,
        Guid exerciseId,
        string exerciseName,
        int sets,
        int reps,
        float weight,
        WeightUnit weightUnit
    ) => new(createdBy, exerciseId, exerciseName, sets, reps, weight, weightUnit);

    public void Update(
        Guid updatedBy,
        Guid? exerciseId = null,
        string? exerciseName = null,
        int? sets = null,
        int? reps = null,
        float? weight = null,
        WeightUnit? weightUnit = null
    )
    {
        if (updatedBy.Equals(Guid.Empty))
            throw new ArgumentException("Workout UpdatedBy cannot be empty.", nameof(updatedBy));

        UpdatedBy = updatedBy;
        UpdatedAtUtc = DateTime.UtcNow;

        if (exerciseId.HasValue && exerciseName is not null)
        {
            ExerciseId = exerciseId.Value;
            ExerciseName = exerciseName;
        }
        if (sets.HasValue)
            Sets = sets.Value;

        if (reps.HasValue)
            Reps = reps.Value;

        if (weight.HasValue)
            Weight = weight.Value;

        if (weightUnit.HasValue)
            WeightUnit = weightUnit.Value;

        Validate(this, new WorkoutExerciseValidator());
    }
}
