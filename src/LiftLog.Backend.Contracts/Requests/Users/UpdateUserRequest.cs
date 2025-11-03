using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Users;
using LiftLog.Backend.Core.Enums;
using MediatR;

namespace LiftLog.Backend.Contracts.Requests.Users;

public record UpdateUserRequest(
    UserType? Type = null,
    Gender? Gender = null,
    HeightUnit? HeightUnit = null,
    WeightUnit? WeightUnit = null,
    Guid? TeacherId = null,
    string? FirstName = null,
    string? LastName = null,
    string? Cpf = null,
    string? PhoneNumber = null,
    string? Email = null,
    float? Height = null,
    float? Weight = null
) : IRequest<Response<UserResponse>>;
