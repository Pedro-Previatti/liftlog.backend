using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Workouts;
using MediatR;

namespace LiftLog.Backend.Contracts.Requests.Workouts;

public record FindWorkoutsRequest(Guid? WorkoutId = null, string? Search = null)
    : IRequest<Response<List<WorkoutResponse>>>;
