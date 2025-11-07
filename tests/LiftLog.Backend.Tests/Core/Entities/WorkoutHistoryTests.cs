using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Tests.Core.Entities;

public class WorkoutHistoryTests
{
    private readonly Guid _createdBy = Guid.NewGuid();
    private readonly Guid _workoutId = Guid.NewGuid();
    private readonly Guid _emptyGuid = Guid.Empty;
    private readonly List<Guid> _exercisesGuids = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];

    [Fact(DisplayName = "WorkoutHistory => Create() Returns Success")]
    [Trait("Category", "Entity")]
    public void WorkoutHistory_Create_ReturnsSuccess()
    {
        var workoutHistory = WorkoutHistory.Create(_createdBy, _workoutId, _exercisesGuids);

        Assert.True(workoutHistory.Valid);
        Assert.Equal(_createdBy, workoutHistory.CreatedBy);
        Assert.Equal(_workoutId, workoutHistory.WorkoutId);
        Assert.Equal(_exercisesGuids, workoutHistory.WorkoutExercisesId);
    }

    [Fact(DisplayName = "WorkoutHistory => Create() Returns Invalid")]
    [Trait("Category", "Entity")]
    public void WorkoutHistory_Create_ReturnsInvalid()
    {
        var workoutHistory = WorkoutHistory.Create(_emptyGuid, _emptyGuid, [_emptyGuid]);

        Assert.True(workoutHistory.Invalid);
        CollectionAssert.IsNotEmpty(workoutHistory.GetNotifications());
        var workoutHistoryNotifications = workoutHistory.GetNotifications();
        Assert.Contains(
            workoutHistoryNotifications,
            x => x.Message == "CreatedBy must be a valid Guid."
        );
        Assert.Contains(
            workoutHistoryNotifications,
            x => x.Message == "WorkoutExercisesId must be a valid list of Guids."
        );
        Assert.Contains(
            workoutHistoryNotifications,
            x => x.Message == "WorkoutId must be a valid Guid."
        );
    }
}
