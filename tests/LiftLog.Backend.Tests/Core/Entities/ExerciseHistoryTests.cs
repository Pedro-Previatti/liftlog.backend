using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Enums;

namespace LiftLog.Backend.Tests.Core.Entities;

public class ExerciseHistoryTests
{
    private static readonly Guid CreatedByGuid = Guid.NewGuid();
    private static readonly Guid ExerciseIdGuid = Guid.NewGuid();
    private static readonly Guid GuidEmpty = Guid.Empty;
    private const WeightUnit Unit = WeightUnit.Kilograms;
    private const WeightUnit InvalidUnit = (WeightUnit)int.MaxValue;
    private const int PositiveInteger = int.MaxValue;
    private const int NegativeInteger = int.MinValue;
    private const float PositiveFloat = float.MaxValue;
    private const float NegativeFloat = float.MinValue;
    private const float NaNFloat = float.NaN;
    private const float InfinityFloat = float.PositiveInfinity;
    private const float NegativeInfinityFloat = float.NegativeInfinity;

    [Fact(DisplayName = "ExerciseHistory => Create() Returns Success")]
    [Trait("Category", "Entity")]
    public void ExerciseHistory_Create_ReturnsSuccess()
    {
        var exerciseHist = ExerciseHistory.Create(
            CreatedByGuid,
            ExerciseIdGuid,
            Unit,
            PositiveInteger,
            PositiveInteger,
            PositiveFloat
        );

        Assert.NotNull(exerciseHist);
        Assert.True(exerciseHist.Valid);
        Assert.Equal(CreatedByGuid, exerciseHist.CreatedBy);
        Assert.Equal(ExerciseIdGuid, exerciseHist.ExerciseId);
        Assert.Equal(Unit, exerciseHist.WeightUnit);
        Assert.Equal(PositiveInteger, exerciseHist.Set);
        Assert.Equal(PositiveInteger, exerciseHist.Reps);
        Assert.Equal(PositiveFloat, exerciseHist.Weight);
        Assert.True(exerciseHist.Valid);
        CollectionAssert.IsEmpty(exerciseHist.GetNotifications());
    }

    [Theory(DisplayName = "ExerciseHistory => Create() Returns Invalid")]
    [Trait("Category", "Entity")]
    [InlineData(NegativeFloat)]
    [InlineData(NaNFloat)]
    [InlineData(InfinityFloat)]
    [InlineData(NegativeInfinityFloat)]
    public void ExerciseHistory_Create_ReturnsInvalid(float weight)
    {
        var exerciseHist = ExerciseHistory.Create(
            GuidEmpty,
            GuidEmpty,
            InvalidUnit,
            NegativeInteger,
            NegativeInteger,
            weight
        );

        Assert.True(exerciseHist.Invalid);
        CollectionAssert.IsNotEmpty(exerciseHist.GetNotifications());
        var exerciseHistNotifications = exerciseHist.GetNotifications();
        Assert.Contains(
            exerciseHistNotifications,
            x => x.Message == "CreatedBy must be a valid Guid."
        );
        Assert.Contains(
            exerciseHistNotifications,
            x => x.Message == "ExerciseId must be a valid Guid."
        );
        Assert.Contains(
            exerciseHistNotifications,
            x => x.Message == "WeightUnit must be a valid enum."
        );
        Assert.Contains(
            exerciseHistNotifications,
            x => x.Message == "Set must be a valid positive integer."
        );
        Assert.Contains(
            exerciseHistNotifications,
            x => x.Message == "Reps must be a valid positive integer."
        );
        Assert.Contains(
            exerciseHistNotifications,
            x => x.Message == "Weight must be a valid positive number."
        );
    }
}
