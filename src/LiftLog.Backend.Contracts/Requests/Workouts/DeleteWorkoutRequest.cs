using LiftLog.Backend.Contracts.Responses;
using MediatR;

namespace LiftLog.Backend.Contracts.Requests.Workouts;

public record DeleteWorkoutRequest(Guid Id) : IRequest<Response<bool>>;
