using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Validators;

namespace LiftLog.Backend.Core.Entities;

public class Exercise : BaseEntity
{
    public required List<Guid> MuscleGroupIds { get; init; }

    public required string Name { get; init; }

    protected Exercise() { }

    [SetsRequiredMembers]
    private Exercise(List<Guid> muscleGroupIds, string name)
    {
        MuscleGroupIds = muscleGroupIds;
        Name = name;

        Validate(this, new ExerciseValidator());
    }

    public static Exercise Create(List<Guid> muscleGroupIds, string name) =>
        new(muscleGroupIds, name);
}
