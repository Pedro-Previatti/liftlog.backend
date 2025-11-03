using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Auth;
using LiftLog.Backend.Core.Enums;
using MediatR;

namespace LiftLog.Backend.Contracts.Requests.Auth;

public record RegisterRequest(
    UserType Type,
    string FirstName,
    string LastName,
    string Cpf,
    string PhoneNumber,
    string Email,
    string Password,
    Guid? TeacherId = null
) : IRequest<Response<AuthResponse>>;
