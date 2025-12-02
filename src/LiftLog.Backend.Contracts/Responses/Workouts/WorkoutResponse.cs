using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Contracts.Responses.Workouts;

public class WorkoutResponse
{
    public required Guid Id { get; set; }
    public required Guid CreatedBy { get; set; }
    public required DateTime UpdatedAtUtc { get; set; }
    public required string Name { get; set; }
    public required DayOfWeek DayOfWeek { get; set; }
    public required List<WorkoutExerciseResponse> Exercises { get; set; }
    public required List<Guid> HasAccess { get; set; }

    [SetsRequiredMembers]
    private WorkoutResponse(
        Guid id,
        Guid createdBy,
        DateTime updatedAtUtc,
        string name,
        DayOfWeek dayOfWeek,
        List<WorkoutExerciseResponse> exercises,
        List<Guid> hasAccess
    )
    {
        Id = id;
        CreatedBy = createdBy;
        UpdatedAtUtc = updatedAtUtc;
        Name = name;
        DayOfWeek = dayOfWeek;
        Exercises = exercises;
        HasAccess = hasAccess;
    }

    public static WorkoutResponse FromEntities(
        Workout workout,
        List<WorkoutExerciseResponse> exercises
    ) =>
        new(
            workout.Id,
            workout.CreatedBy,
            workout.UpdatedAtUtc,
            workout.Name,
            workout.DayOfWeek,
            exercises,
            workout.CreatedForUserIds
        );
}
