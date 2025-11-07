using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Contracts.Responses.Exercises;

public class ExerciseResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required List<string> MuscleGroups { get; init; }

    [SetsRequiredMembers]
    private ExerciseResponse(Exercise exercise, List<string> muscleGroups)
    {
        Id = exercise.Id;
        Name = exercise.Name;
        MuscleGroups = muscleGroups;
    }

    public static ExerciseResponse FromEntities(
        Exercise exercise,
        List<MuscleGroup> muscleGroups
    ) => new(exercise, MapMuscleGroups(exercise, muscleGroups));

    private static List<string> MapMuscleGroups(
        Exercise exercise,
        List<MuscleGroup> muscleGroups
    ) =>
        exercise
            .MuscleGroupIds.Select(id => muscleGroups.FirstOrDefault(mg => mg.Id == id)?.Name)
            .Where(name => name is not null)
            .Cast<string>()
            .ToList();
}
