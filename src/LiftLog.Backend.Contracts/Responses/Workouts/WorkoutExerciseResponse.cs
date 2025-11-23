using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Enums;

namespace LiftLog.Backend.Contracts.Responses.Workouts;

public class WorkoutExerciseResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required WeightUnit WeightUnit { get; set; }
    public required int Sets { get; set; }
    public required int Reps { get; set; }
    public required float Weight { get; set; }

    [SetsRequiredMembers]
    private WorkoutExerciseResponse(
        Guid id,
        string name,
        WeightUnit weightUnit,
        int sets,
        int reps,
        float weight
    )
    {
        Id = id;
        Name = name;
        WeightUnit = weightUnit;
        Sets = sets;
        Reps = reps;
        Weight = weight;
    }

    public static WorkoutExerciseResponse FromEntity(WorkoutExercise ex) =>
        new(ex.Id, ex.ExerciseName, ex.WeightUnit, ex.Sets, ex.Reps, ex.Weight);
}
