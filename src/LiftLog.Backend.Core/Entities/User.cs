using System.Diagnostics.CodeAnalysis;
using LiftLog.Backend.Core.Enums;
using LiftLog.Backend.Core.Validators;

namespace LiftLog.Backend.Core.Entities;

public class User : BaseEntity
{
    public required UserType Type { get; set; }
    public Gender? Gender { get; set; }
    public HeightUnit? HeightUnit { get; set; }
    public WeightUnit? WeightUnit { get; set; }

    public Guid? TeacherId { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Cpf { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string SearchText { get; set; }

    public float? Height { get; set; }
    public float? Weight { get; set; }

    protected User() { }

    [SetsRequiredMembers]
    private User(
        UserType type,
        Guid? teacherId,
        string firstName,
        string lastName,
        string cpf,
        string phoneNumber,
        string email,
        string password
    )
    {
        Type = type;
        Gender = Enums.Gender.Undefined;
        TeacherId = teacherId;
        FirstName = firstName;
        LastName = lastName;
        Cpf = cpf;
        PhoneNumber = phoneNumber;
        Email = email;
        Password = password;
        SearchText = $"{FirstName} {LastName} {Cpf} {PhoneNumber} {Email}";

        Validate(this, new UserValidator());
    }

    public static User Create(
        UserType type,
        Guid? teacherId,
        string firstName,
        string lastName,
        string cpf,
        string phoneNumber,
        string email,
        string password
    ) => new(type, teacherId, firstName, lastName, cpf, phoneNumber, email, password);

    public User Update(
        UserType? type = null,
        Gender? gender = null,
        HeightUnit? heightUnit = null,
        WeightUnit? weightUnit = null,
        Guid? teacherId = null,
        string? firstName = null,
        string? lastName = null,
        string? cpf = null,
        string? phoneNumber = null,
        string? email = null,
        string? password = null,
        float? height = null,
        float? weight = null
    )
    {
        UpdatedAtUtc = DateTime.UtcNow;

        if (type.HasValue)
            Type = type.Value;

        if (gender.HasValue)
            Gender = gender.Value;

        if (heightUnit.HasValue)
            HeightUnit = heightUnit.Value;

        if (weightUnit.HasValue)
            WeightUnit = weightUnit.Value;

        if (teacherId.HasValue)
            TeacherId = teacherId.Value;

        if (!string.IsNullOrWhiteSpace(firstName))
            FirstName = firstName;

        if (!string.IsNullOrWhiteSpace(lastName))
            LastName = lastName;

        if (!string.IsNullOrWhiteSpace(cpf))
            Cpf = cpf;

        if (!string.IsNullOrWhiteSpace(phoneNumber))
            PhoneNumber = phoneNumber;

        if (!string.IsNullOrWhiteSpace(email))
            Email = email;

        if (!string.IsNullOrWhiteSpace(password))
            Password = password;

        if (height.HasValue)
            Height = height.Value;

        if (weight.HasValue)
            Weight = weight.Value;

        SearchText = $"{FirstName} {LastName} {Cpf} {PhoneNumber} {Email}";

        Validate(this, new UserValidator());

        return this;
    }
}
