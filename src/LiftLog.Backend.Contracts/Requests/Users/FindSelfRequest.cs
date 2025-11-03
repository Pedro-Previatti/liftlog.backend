using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Users;
using MediatR;

namespace LiftLog.Backend.Contracts.Requests.Users;

public record FindSelfRequest : IRequest<Response<SelfResponse>>;
