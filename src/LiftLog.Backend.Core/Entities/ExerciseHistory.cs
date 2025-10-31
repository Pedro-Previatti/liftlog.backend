using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Enums;
using LiftLog.Backend.Core.Validators;

namespace LiftLog.Backend.Core.Entities;

public class ExerciseHistory : BaseEntity
{
    public required Guid ExerciseId { get; init; }

    public required WeightUnit WeightUnit { get; init; }

    public required int Set { get; init; }
    public required int Reps { get; init; }

    public required float Weight { get; init; }

    protected ExerciseHistory() { }

    [SetsRequiredMembers]
    private ExerciseHistory(
        Guid createdBy,
        Guid exerciseId,
        WeightUnit weightUnit,
        int set,
        int reps,
        float weight
    )
    {
        CreatedBy = createdBy;

        ExerciseId = exerciseId;
        WeightUnit = weightUnit;
        Set = set;
        Reps = reps;
        Weight = weight;

        Validate(this, new ExerciseHistoryValidator());
    }

    public static ExerciseHistory Create(
        Guid createdBy,
        Guid exerciseId,
        WeightUnit weightUnit,
        int set,
        int reps,
        float weight
    ) => new(createdBy, exerciseId, weightUnit, set, reps, weight);
}
