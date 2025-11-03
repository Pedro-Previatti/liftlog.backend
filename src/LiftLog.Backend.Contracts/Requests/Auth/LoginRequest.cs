using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Auth;
using MediatR;

namespace LiftLog.Backend.Contracts.Requests.Auth;

public record LoginRequest(string Email, string Password) : IRequest<Response<AuthResponse>>;
