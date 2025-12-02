using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Tests.Core.Entities;

public class WorkoutTests
{
    private readonly Guid _guidEmpty = Guid.Empty;
    private readonly Guid _createdBy = Guid.NewGuid();
    private readonly Guid _newExGuid = Guid.NewGuid();
    private readonly Guid _newUserGuid = Guid.NewGuid();
    private readonly List<Guid> _testExListIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
    private readonly List<Guid> _testUserListIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
    private readonly string _stringEmpty = string.Empty;
    private const string SmallName = "sn";
    private const string Name = "Workout";
    private const string NewName = "NewName";
    private const string LargeName = "Large Name Would Be Unusual For A Workout";
    private const DayOfWeek CorrectDayOfWeek = DayOfWeek.Monday;
    private const DayOfWeek IncorrectDayOfWeek = (DayOfWeek)int.MaxValue;

    [Fact(DisplayName = "Workout => Create() Returns Success")]
    [Trait("Category", "Entity")]
    public void Workout_Create_ReturnsSuccess()
    {
        var workout = Workout.Create(
            _testExListIds,
            _testUserListIds,
            _createdBy,
            Name,
            CorrectDayOfWeek
        );

        Assert.NotNull(workout);
        Assert.True(workout.Valid);
        Assert.Equal(_createdBy, workout.CreatedBy);
        Assert.Equal(_createdBy, workout.UpdatedBy);
        Assert.Equal(Name, workout.Name);
        foreach (var guid in _testExListIds)
        {
            Assert.Contains(workout.WorkoutExerciseIds, x => x == guid);
        }
        foreach (var guid in _testUserListIds)
        {
            Assert.Contains(workout.CreatedForUserIds, x => x == guid);
        }
    }

    [Fact(DisplayName = "Workout => Create() Returns Failure")]
    [Trait("Category", "Entity")]
    public void Workout_Create_ReturnsFailure()
    {
        var workoutOne = Workout.Create([], [], _guidEmpty, _stringEmpty, IncorrectDayOfWeek);
        var workoutTwo = Workout.Create(
            _testExListIds,
            _testUserListIds,
            _createdBy,
            SmallName,
            IncorrectDayOfWeek
        );
        var workoutThree = Workout.Create(
            _testExListIds,
            _testUserListIds,
            _createdBy,
            LargeName,
            IncorrectDayOfWeek
        );
        var workoutFour = Workout.Create(
            [_guidEmpty],
            [_guidEmpty],
            _createdBy,
            Name,
            IncorrectDayOfWeek
        );

        Assert.True(workoutOne.Invalid);
        CollectionAssert.IsNotEmpty(workoutOne.GetNotifications());
        var workoutOneNotifications = workoutOne.GetNotifications();
        Assert.Contains(
            workoutOneNotifications,
            x => x.Message == "CreatedBy must be a valid Guid."
        );
        Assert.Contains(
            workoutOneNotifications,
            x => x.Message == "WorkoutExerciseIds must be a valid list of Guids."
        );
        Assert.Contains(
            workoutOneNotifications,
            x => x.Message == "CreatedForUserIds must be a valid list of Guids."
        );
        Assert.Contains(workoutOneNotifications, x => x.Message == "Name is required.");
        Assert.Contains(
            workoutOneNotifications,
            x => x.Message == "Day of the week must be a valid enum value."
        );

        Assert.True(workoutTwo.Invalid);
        CollectionAssert.IsNotEmpty(workoutTwo.GetNotifications());
        var workoutTwoNotifications = workoutTwo.GetNotifications();
        Assert.Contains(
            workoutTwoNotifications,
            x => x.Message == "Name must have more than 2 characters."
        );
        Assert.Contains(
            workoutTwoNotifications,
            x => x.Message == "Day of the week must be a valid enum value."
        );

        Assert.True(workoutThree.Invalid);
        CollectionAssert.IsNotEmpty(workoutThree.GetNotifications());
        var workoutThreeNotifications = workoutThree.GetNotifications();
        Assert.Contains(
            workoutThreeNotifications,
            x => x.Message == "Name must be up to 30 characters."
        );
        Assert.Contains(
            workoutThreeNotifications,
            x => x.Message == "Day of the week must be a valid enum value."
        );

        Assert.True(workoutFour.Invalid);
        CollectionAssert.IsNotEmpty(workoutFour.GetNotifications());
        var workoutFourNotifications = workoutFour.GetNotifications();
        Assert.Contains(
            workoutFourNotifications,
            x => x.Message == "WorkoutExerciseIds must be a valid list of Guids."
        );
        Assert.Contains(
            workoutFourNotifications,
            x => x.Message == "CreatedForUserIds must be a valid list of Guids."
        );
        Assert.Contains(
            workoutFourNotifications,
            x => x.Message == "Day of the week must be a valid enum value."
        );
    }

    [Fact(DisplayName = "Workout => Update() Returns Success")]
    [Trait("Category", "Entity")]
    public void Workout_Update_ReturnsSuccess()
    {
        var workout = Workout.Create(
            _testExListIds,
            _testUserListIds,
            _createdBy,
            Name,
            CorrectDayOfWeek
        );
        var existingWorkoutGuid = workout.WorkoutExerciseIds.First();
        var existingUserGuid = workout.CreatedForUserIds.First();
        workout.Update(
            _createdBy,
            [existingWorkoutGuid, _newExGuid],
            [existingUserGuid, _newUserGuid],
            NewName
        );

        Assert.NotNull(workout);
        Assert.True(workout.Valid);
        Assert.Equal(_createdBy, workout.CreatedBy);
        Assert.Equal(_createdBy, workout.UpdatedBy);
        Assert.Equal(NewName, workout.Name);
        foreach (var guid in _testExListIds)
        {
            Assert.Single(workout.WorkoutExerciseIds, x => x == guid);
        }
        foreach (var guid in _testUserListIds)
        {
            Assert.Single(workout.CreatedForUserIds, x => x == guid);
        }
        Assert.Contains(_newExGuid, workout.WorkoutExerciseIds);
        Assert.Contains(_newUserGuid, workout.CreatedForUserIds);
    }

    [Fact(DisplayName = "Workout => Update() ArgumentException ")]
    [Trait("Category", "Entity")]
    public void Workout_Update_ThrowsArgumentException()
    {
        var workout = Workout.Create(
            _testExListIds,
            _testUserListIds,
            _createdBy,
            Name,
            CorrectDayOfWeek
        );

        Assert.Throws<ArgumentException>(() => workout.Update(Guid.Empty, _testExListIds));
    }

    [Fact(DisplayName = "Workout => Update() Returns Invalid")]
    [Trait("Category", "Entity")]
    public void Workout_Update_ReturnsInvalid()
    {
        var workout = Workout.Create(
            _testExListIds,
            _testUserListIds,
            _createdBy,
            Name,
            CorrectDayOfWeek
        );
        workout.Update(_createdBy, [_guidEmpty], [_guidEmpty], LargeName, IncorrectDayOfWeek);

        Assert.True(workout.Invalid);
        CollectionAssert.IsNotEmpty(workout.GetNotifications());
        var workoutNotifications = workout.GetNotifications();
        Assert.Contains(
            workoutNotifications,
            x => x.Message == "WorkoutExerciseIds must be a valid list of Guids."
        );
        Assert.Contains(
            workoutNotifications,
            x => x.Message == "CreatedForUserIds must be a valid list of Guids."
        );
        Assert.Contains(
            workoutNotifications,
            x => x.Message == "Name must be up to 30 characters."
        );
        Assert.Contains(
            workoutNotifications,
            x => x.Message == "Day of the week must be a valid enum value."
        );
    }

    [Fact(DisplayName = "Workout => RemoveExerciseFromWorkout() Returns Success")]
    [Trait("Category", "Entity")]
    public void Workout_RemoveExerciseFromWorkout_ReturnsSuccess()
    {
        var workout = Workout.Create(
            _testExListIds,
            _testUserListIds,
            _createdBy,
            Name,
            CorrectDayOfWeek
        );
        var removeGuid = _testExListIds[0];
        workout.RemoveExerciseFromWorkout(_createdBy, removeGuid);

        Assert.True(workout.Valid);
        Assert.Equal(_createdBy, workout.CreatedBy);
        Assert.Equal(_createdBy, workout.UpdatedBy);
        Assert.Equal(Name, workout.Name);
        Assert.DoesNotContain(removeGuid, workout.WorkoutExerciseIds);
    }

    [Fact(DisplayName = "Workout => RemoveExerciseFromWorkout() ArgumentException ")]
    [Trait("Category", "Entity")]
    public void Workout_RemoveExerciseFromWorkout_ThrowsArgumentException()
    {
        var workout = Workout.Create(
            _testExListIds,
            _testUserListIds,
            _createdBy,
            Name,
            CorrectDayOfWeek
        );

        Assert.Throws<ArgumentException>(() =>
            workout.RemoveExerciseFromWorkout(Guid.Empty, _newExGuid)
        );
    }

    [Fact(DisplayName = "Workout => RemoveUserFromWorkout() Returns Success")]
    [Trait("Category", "Entity")]
    public void Workout_RemoveUserFromWorkout_ReturnsSuccess()
    {
        var workout = Workout.Create(
            _testExListIds,
            _testUserListIds,
            _createdBy,
            Name,
            CorrectDayOfWeek
        );
        var removeGuid = _testUserListIds[0];
        workout.RemoveUserFromWorkout(_createdBy, removeGuid);

        Assert.True(workout.Valid);
        Assert.Equal(_createdBy, workout.CreatedBy);
        Assert.Equal(_createdBy, workout.UpdatedBy);
        Assert.Equal(Name, workout.Name);
        Assert.DoesNotContain(removeGuid, workout.CreatedForUserIds);
    }

    [Fact(DisplayName = "Workout => RemoveUserFromWorkout() ArgumentException ")]
    [Trait("Category", "Entity")]
    public void Workout_RemoveUserFromWorkout_ThrowsArgumentException()
    {
        var workout = Workout.Create(
            _testExListIds,
            _testUserListIds,
            _createdBy,
            Name,
            CorrectDayOfWeek
        );

        Assert.Throws<ArgumentException>(() =>
            workout.RemoveUserFromWorkout(Guid.Empty, _newExGuid)
        );
    }
}
