using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Tests.Core.Entities;

public class ExerciseTests
{
    private readonly List<Guid> _testMgGuids = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
    private const string SmallExerciseName = "ex";
    private const string ExerciseName = "Exercise Test";
    private const string HugeExerciseName = "Huge Exercise Name With More Than Fifty Characters!";

    [Fact(DisplayName = "Exercise => Create() Returns Success")]
    [Trait("Category", "Entity")]
    public void Exercise_Create_ReturnsSuccess()
    {
        var exercise = Exercise.Create(_testMgGuids, ExerciseName);

        Assert.NotNull(exercise);
        Assert.True(exercise.Valid);
        Assert.Equal(ExerciseName, exercise.Name);
        Assert.Equal(_testMgGuids, exercise.MuscleGroupIds);
        foreach (var guid in _testMgGuids)
        {
            Assert.Single(exercise.MuscleGroupIds, x => x == guid);
        }
        Assert.True(exercise.Valid);
        CollectionAssert.IsEmpty(exercise.GetNotifications());
    }

    [Fact(DisplayName = "Exercise => Create() Returns Invalid")]
    [Trait("Category", "Entity")]
    public void Exercise_Create_ReturnsInvalid()
    {
        var exerciseOne = Exercise.Create([], string.Empty);
        var exerciseTwo = Exercise.Create([], SmallExerciseName);
        var exerciseThree = Exercise.Create([], ExerciseName);
        var exerciseFour = Exercise.Create([], HugeExerciseName);
        var exerciseFive = Exercise.Create(_testMgGuids, string.Empty);
        var exerciseSix = Exercise.Create([Guid.Empty], ExerciseName);

        Assert.True(exerciseOne.Invalid);
        CollectionAssert.IsNotEmpty(exerciseOne.GetNotifications());
        var exOneNotifications = exerciseOne.GetNotifications();
        Assert.Contains(exOneNotifications, x => x.Message == "Name is required.");
        Assert.Contains(
            exOneNotifications,
            x => x.Message == "MuscleGroupIds must be a valid list of Guids."
        );

        Assert.True(exerciseTwo.Invalid);
        CollectionAssert.IsNotEmpty(exerciseTwo.GetNotifications());
        var exTwoNotifications = exerciseTwo.GetNotifications();
        Assert.Contains(
            exTwoNotifications,
            x => x.Message == "Name must have more than 2 characters."
        );
        Assert.Contains(
            exTwoNotifications,
            x => x.Message == "MuscleGroupIds must be a valid list of Guids."
        );

        Assert.True(exerciseThree.Invalid);
        CollectionAssert.IsNotEmpty(exerciseThree.GetNotifications());
        var exThreeNotifications = exerciseThree.GetNotifications();
        Assert.Contains(
            exThreeNotifications,
            x => x.Message == "MuscleGroupIds must be a valid list of Guids."
        );

        Assert.True(exerciseFour.Invalid);
        CollectionAssert.IsNotEmpty(exerciseFour.GetNotifications());
        var exFourNotifications = exerciseFour.GetNotifications();
        Assert.Contains(exFourNotifications, x => x.Message == "Name must be up to 50 characters.");
        Assert.Contains(
            exFourNotifications,
            x => x.Message == "MuscleGroupIds must be a valid list of Guids."
        );

        Assert.True(exerciseFive.Invalid);
        CollectionAssert.IsNotEmpty(exerciseFive.GetNotifications());
        var exFiveNotifications = exerciseFive.GetNotifications();
        Assert.Contains(exFiveNotifications, x => x.Message == "Name is required.");

        Assert.True(exerciseSix.Invalid);
        CollectionAssert.IsNotEmpty(exerciseSix.GetNotifications());
        var exSixNotifications = exerciseSix.GetNotifications();
        Assert.Contains(
            exSixNotifications,
            x => x.Message == "MuscleGroupIds must be a valid list of Guids."
        );
    }
}
