using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.MuscleGroups;
using MediatR;

namespace LiftLog.Backend.Contracts.Requests.MuscleGroups;

public record FindMuscleGroupsRequest(string? Search = null)
    : IRequest<Response<List<MuscleGroupResponse>>>;
