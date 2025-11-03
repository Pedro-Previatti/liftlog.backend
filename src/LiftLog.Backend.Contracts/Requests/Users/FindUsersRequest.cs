using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Users;
using MediatR;

namespace LiftLog.Backend.Contracts.Requests.Users;

public record FindUsersRequest(Guid? Id = null, string? Search = null)
    : IRequest<Response<List<UserResponse>>>;
