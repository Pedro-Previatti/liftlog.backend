using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Tests.Core.Entities;

public class MuscleGroupTests
{
    private const string SmallMuscleGroupName = "mg";
    private const string MuscleGroupName = "Muscle Group";
    private const string HugeMuscleGroupName =
        "Huge Muscle Group Name With More Than Fifty Characters!";

    [Fact(DisplayName = "MuscleGroup => Create() Returns Success")]
    [Trait("Category", "Entity")]
    public void Exercise_Create_ReturnsSuccess()
    {
        var muscleGroup = MuscleGroup.Create(MuscleGroupName);

        Assert.NotNull(muscleGroup);
        Assert.True(muscleGroup.Valid);
        Assert.Equal(MuscleGroupName, muscleGroup.Name);
        Assert.True(muscleGroup.Valid);
        CollectionAssert.IsEmpty(muscleGroup.GetNotifications());
    }

    [Fact(DisplayName = "MuscleGroup => Create() Returns Invalid")]
    [Trait("Category", "Entity")]
    public void MuscleGroup_Create_ReturnsInvalid()
    {
        var muscleGroupOne = MuscleGroup.Create(string.Empty);
        var muscleGroupTwo = MuscleGroup.Create(SmallMuscleGroupName);
        var muscleGroupThree = MuscleGroup.Create(HugeMuscleGroupName);

        Assert.True(muscleGroupOne.Invalid);
        CollectionAssert.IsNotEmpty(muscleGroupOne.GetNotifications());
        var muscleGroupOneNotifications = muscleGroupOne.GetNotifications();
        Assert.Contains(muscleGroupOneNotifications, x => x.Message == "Name is required.");

        Assert.True(muscleGroupTwo.Invalid);
        CollectionAssert.IsNotEmpty(muscleGroupTwo.GetNotifications());
        var muscleGroupTwoNotifications = muscleGroupTwo.GetNotifications();
        Assert.Contains(
            muscleGroupTwoNotifications,
            x => x.Message == "Name must have more than 2 characters."
        );

        Assert.True(muscleGroupThree.Invalid);
        CollectionAssert.IsNotEmpty(muscleGroupThree.GetNotifications());
        var muscleGroupThreeNotifications = muscleGroupThree.GetNotifications();
        Assert.Contains(
            muscleGroupThreeNotifications,
            x => x.Message == "Name must be up to 30 characters."
        );
    }
}
