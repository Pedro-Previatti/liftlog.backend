using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Enums;

namespace LiftLog.Backend.Tests.Core.Entities;

public class WorkoutExerciseTests
{
    private readonly Guid _createdBy = Guid.NewGuid();
    private readonly Guid _exerciseId = Guid.NewGuid();
    private readonly Guid _emptyGuid = Guid.Empty;
    private const string ExerciseName = "Name";
    private const WeightUnit KilogramUnit = WeightUnit.Kilograms;
    private const WeightUnit PoundUnit = WeightUnit.Pounds;
    private const WeightUnit InvalidUnit = (WeightUnit)int.MaxValue;
    private const int PositiveInteger = int.MaxValue;
    private const int NegativeInteger = int.MinValue;
    private const float PositiveFloat = float.MaxValue;
    private const float NegativeFloat = float.MinValue;
    private const float NaNFloat = float.NaN;
    private const float InfinityFloat = float.PositiveInfinity;
    private const float NegativeInfinityFloat = float.NegativeInfinity;

    [Fact(DisplayName = "WorkoutExercise => Create() Returns Success")]
    [Trait("Category", "Entity")]
    public void WorkoutExercise_Create_ReturnsSuccess()
    {
        var workoutExercise = WorkoutExercise.Create(
            _createdBy,
            _exerciseId,
            ExerciseName,
            PositiveInteger,
            PositiveInteger,
            PositiveFloat,
            KilogramUnit
        );

        Assert.True(workoutExercise.Valid);
        Assert.Equal(_createdBy, workoutExercise.CreatedBy);
        Assert.Equal(_createdBy, workoutExercise.UpdatedBy);
        Assert.Equal(_exerciseId, workoutExercise.ExerciseId);
        Assert.Equal(PositiveInteger, workoutExercise.Sets);
        Assert.Equal(PositiveInteger, workoutExercise.Reps);
        Assert.Equal(PositiveFloat, workoutExercise.Weight);
        Assert.Equal(KilogramUnit, workoutExercise.WeightUnit);
    }

    [Theory(DisplayName = "WorkoutExercise => Create() Returns Invalid")]
    [Trait("Category", "Entity")]
    [InlineData(NegativeFloat)]
    [InlineData(NaNFloat)]
    [InlineData(InfinityFloat)]
    [InlineData(NegativeInfinityFloat)]
    public void WorkoutExercise_Create_ReturnsInvalid(float weight)
    {
        var workoutExercise = WorkoutExercise.Create(
            _emptyGuid,
            _emptyGuid,
            ExerciseName,
            NegativeInteger,
            NegativeInteger,
            weight,
            InvalidUnit
        );

        Assert.True(workoutExercise.Invalid);
        CollectionAssert.IsNotEmpty(workoutExercise.GetNotifications());
        var workoutExerciseNotifications = workoutExercise.GetNotifications();
        Assert.Contains(
            workoutExerciseNotifications,
            x => x.Message == "CreatedBy must be a valid Guid."
        );
        Assert.Contains(
            workoutExerciseNotifications,
            x => x.Message == "UpdatedBy must be a valid Guid."
        );
        Assert.Contains(
            workoutExerciseNotifications,
            x => x.Message == "ExerciseId must be a valid Guid."
        );
        Assert.Contains(
            workoutExerciseNotifications,
            x => x.Message == "WeightUnit must be a valid enum value."
        );
        Assert.Contains(
            workoutExerciseNotifications,
            x => x.Message == "Sets must be a valid positive integer."
        );
        Assert.Contains(
            workoutExerciseNotifications,
            x => x.Message == "Reps must be a valid positive integer."
        );
        Assert.Contains(
            workoutExerciseNotifications,
            x => x.Message == "Weight must be a valid positive number."
        );
    }

    [Fact(DisplayName = "WorkoutExercise => Update() Returns Success")]
    [Trait("Category", "Entity")]
    public void WorkoutExercise_Update_ReturnsSuccess()
    {
        var workoutExercise = WorkoutExercise.Create(
            _createdBy,
            _exerciseId,
            ExerciseName,
            PositiveInteger,
            PositiveInteger,
            PositiveFloat,
            KilogramUnit
        );
        workoutExercise.Update(
            _createdBy,
            _exerciseId,
            ExerciseName,
            PositiveInteger - 1,
            PositiveInteger - 1,
            PositiveFloat - 1,
            PoundUnit
        );

        Assert.True(workoutExercise.Valid);
        Assert.Equal(_createdBy, workoutExercise.CreatedBy);
        Assert.Equal(_createdBy, workoutExercise.UpdatedBy);
        Assert.Equal(_exerciseId, workoutExercise.ExerciseId);
        Assert.Equal(PositiveInteger - 1, workoutExercise.Sets);
        Assert.Equal(PositiveInteger - 1, workoutExercise.Reps);
        Assert.Equal(PositiveFloat - 1, workoutExercise.Weight);
        Assert.Equal(PoundUnit, workoutExercise.WeightUnit);
    }

    [Fact(DisplayName = "WorkoutExercise => Update() Returns ArgumentException")]
    [Trait("Category", "Entity")]
    public void WorkoutExercise_Update_ArgumentException()
    {
        var workoutExercise = WorkoutExercise.Create(
            _createdBy,
            _exerciseId,
            ExerciseName,
            PositiveInteger,
            PositiveInteger,
            PositiveFloat,
            KilogramUnit
        );

        Assert.Throws<ArgumentException>(() =>
            workoutExercise.Update(_emptyGuid, weightUnit: KilogramUnit)
        );
    }

    [Theory(DisplayName = "WorkoutExercise => Update() Returns Invalid")]
    [Trait("Category", "Entity")]
    [InlineData(NegativeFloat)]
    [InlineData(NaNFloat)]
    [InlineData(InfinityFloat)]
    [InlineData(NegativeInfinityFloat)]
    public void WorkoutExercise_Update_ReturnsInvalid(float weight)
    {
        var workoutExercise = WorkoutExercise.Create(
            _createdBy,
            _exerciseId,
            ExerciseName,
            PositiveInteger,
            PositiveInteger,
            PositiveFloat,
            KilogramUnit
        );
        workoutExercise.Update(
            _createdBy,
            _emptyGuid,
            ExerciseName,
            NegativeInteger,
            NegativeInteger,
            weight,
            InvalidUnit
        );

        Assert.True(workoutExercise.Invalid);
        Assert.True(workoutExercise.Invalid);
        CollectionAssert.IsNotEmpty(workoutExercise.GetNotifications());
        var workoutExerciseNotifications = workoutExercise.GetNotifications();
        Assert.Contains(
            workoutExerciseNotifications,
            x => x.Message == "ExerciseId must be a valid Guid."
        );
        Assert.Contains(
            workoutExerciseNotifications,
            x => x.Message == "WeightUnit must be a valid enum value."
        );
        Assert.Contains(
            workoutExerciseNotifications,
            x => x.Message == "Sets must be a valid positive integer."
        );
        Assert.Contains(
            workoutExerciseNotifications,
            x => x.Message == "Reps must be a valid positive integer."
        );
        Assert.Contains(
            workoutExerciseNotifications,
            x => x.Message == "Weight must be a valid positive number."
        );
    }
}
