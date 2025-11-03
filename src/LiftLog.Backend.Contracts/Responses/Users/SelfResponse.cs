using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Enums;

namespace LiftLog.Backend.Contracts.Responses.Users;

public class SelfResponse
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public required DateTime UpdatedAtUtc { get; init; }
    public required UserType Type { get; init; }
    public required Gender? Gender { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Cpf { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Email { get; init; }
    public required HeightUnit? HeightUnit { get; init; }
    public required float? Height { get; init; }
    public required WeightUnit? WeightUnit { get; init; }
    public required float? Weight { get; init; }
    public required Guid? TeacherId { get; init; }

    [SetsRequiredMembers]
    private SelfResponse(User user)
    {
        Id = user.Id;
        CreatedAtUtc = user.CreatedAtUtc;
        UpdatedAtUtc = user.UpdatedAtUtc;
        Type = user.Type;
        Gender = user.Gender;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Cpf = user.Cpf;
        PhoneNumber = user.PhoneNumber;
        Email = user.Email;
        HeightUnit = user.HeightUnit;
        Height = user.Height;
        WeightUnit = user.WeightUnit;
        Weight = user.Weight;
        TeacherId = user.TeacherId;
    }

    public static SelfResponse FromEntity(User user) => new(user);
}
