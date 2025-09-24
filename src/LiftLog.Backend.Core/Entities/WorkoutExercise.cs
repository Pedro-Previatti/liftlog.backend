using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Enums;
using LiftLog.Backend.Core.Validators;

namespace LiftLog.Backend.Core.Entities;

public class WorkoutExercise : BaseEntity
{
    public required Guid ExerciseId { get; set; }

    public required WeightUnit WeightUnit { get; set; }

    public required int Sets { get; set; }
    public required int Reps { get; set; }

    public required float Weight { get; set; }

    protected WorkoutExercise() { }

    [SetsRequiredMembers]
    private WorkoutExercise(
        Guid createdBy,
        Guid exerciseId,
        int sets,
        int reps,
        float weight,
        WeightUnit weightUnit
    )
    {
        CreatedBy = createdBy;
        UpdatedBy = createdBy;

        ExerciseId = exerciseId;
        Sets = sets;
        Reps = reps;
        Weight = weight;
        WeightUnit = weightUnit;

        Validate(this, new WorkoutExerciseValidator());
    }

    public static WorkoutExercise Create(
        Guid createdBy,
        Guid exerciseId,
        int sets,
        int reps,
        float weight,
        WeightUnit weightUnit
    ) => new(createdBy, exerciseId, sets, reps, weight, weightUnit);

    public void Update(
        Guid updatedBy,
        Guid? exerciseId = null,
        int? sets = null,
        int? reps = null,
        float? weight = null,
        WeightUnit? weightUnit = null
    )
    {
        UpdatedBy = updatedBy;
        UpdatedAtUtc = DateTime.UtcNow;

        if (exerciseId.HasValue)
            ExerciseId = exerciseId.Value;

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
