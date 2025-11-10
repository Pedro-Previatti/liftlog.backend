using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Enums;

namespace LiftLog.Backend.Contracts.Responses.Workouts;

public class WorkoutExerciseResponse
{
    public required string Name { get; set; }
    public required WeightUnit WeightUnit { get; set; }
    public required int Sets { get; set; }
    public required int Reps { get; set; }

    [SetsRequiredMembers]
    private WorkoutExerciseResponse(string name, WeightUnit weightUnit, int sets, int reps)
    {
        Name = name;
        WeightUnit = weightUnit;
        Sets = sets;
        Reps = reps;
    }

    public static WorkoutExerciseResponse FromEntity(WorkoutExercise ex) =>
        new(ex.ExerciseName, ex.WeightUnit, ex.Sets, ex.Reps);
}
