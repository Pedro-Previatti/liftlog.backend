using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Validators;

namespace LiftLog.Backend.Core.Entities;

public class WorkoutHistory : BaseEntity
{
    public required List<Guid> WorkoutExercisesId { get; set; }

    public required Guid WorkoutId { get; init; }

    protected WorkoutHistory() { }

    [SetsRequiredMembers]
    private WorkoutHistory(Guid createdBy, Guid workoutId, List<Guid> workoutExercisesId)
    {
        CreatedBy = createdBy;

        WorkoutExercisesId = workoutExercisesId;
        WorkoutId = workoutId;

        Validate(this, new WorkoutHistoryValidator());
    }

    public static WorkoutHistory Create(
        Guid createdBy,
        Guid workoutId,
        List<Guid> workoutExercisesId
    ) => new(createdBy, workoutId, workoutExercisesId);
}
