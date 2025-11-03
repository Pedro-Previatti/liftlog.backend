using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Auth;
using MediatR;

namespace LiftLog.Backend.Contracts.Requests.Auth;

public record RefreshTokenRequest(string Token, string RefreshToken)
    : IRequest<Response<AuthResponse>>;
