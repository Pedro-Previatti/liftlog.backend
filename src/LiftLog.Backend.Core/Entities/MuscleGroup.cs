using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Validators;

namespace LiftLog.Backend.Core.Entities;

public class MuscleGroup : BaseEntity
{
    public required string Name { get; set; }

    protected MuscleGroup() { }

    [SetsRequiredMembers]
    private MuscleGroup(string name)
    {
        Name = name;

        Validate(this, new MuscleGroupValidator());
    }

    public static MuscleGroup Create(string name) => new(name);
}
