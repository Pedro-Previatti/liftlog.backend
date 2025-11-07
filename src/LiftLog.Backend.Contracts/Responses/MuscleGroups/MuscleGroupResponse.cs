using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Contracts.Responses.MuscleGroups;

public class MuscleGroupResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }

    [SetsRequiredMembers]
    private MuscleGroupResponse(MuscleGroup muscleGroup)
    {
        Id = muscleGroup.Id;
        Name = muscleGroup.Name;
    }

    public static MuscleGroupResponse FromEntity(MuscleGroup muscleGroup) => new(muscleGroup);
}
