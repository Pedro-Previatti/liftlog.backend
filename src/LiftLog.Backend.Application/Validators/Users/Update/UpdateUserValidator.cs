using FluentValidation;
using LiftLog.Backend.Contracts.Requests.Users;
using LiftLog.Backend.Core.Enums;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Shared;

namespace LiftLog.Backend.Application.Validators.Users.Update;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator()
    {
        When(
            x => x.Type is not null,
            () =>
            {
                RuleFor(x => x.Type)
                    .Must(x => x.HasValue && Enum.IsDefined(typeof(UserType), x))
                    .WithMessage("Type must be a valid enum value.")
                    .Must(x => x.ToString().IsSafeForSqlInput())
                    .WithMessage("Type contains invalid characters.");
            }
        );

        When(
            x => x.Gender is not null,
            () =>
            {
                RuleFor(x => x.Gender)
                    .Must(x => x.HasValue && Enum.IsDefined(typeof(Gender), x))
                    .WithMessage("Gender must be a valid enum value.")
                    .Must(x => x.ToString().IsSafeForSqlInput())
                    .WithMessage("Gender contains invalid characters.");
            }
        );

        When(
            x => x.HeightUnit is not null,
            () =>
            {
                RuleFor(x => x.HeightUnit)
                    .Must(x => x.HasValue && Enum.IsDefined(typeof(HeightUnit), x))
                    .WithMessage("HeightUnit must be a valid enum value.")
                    .Must(x => x.ToString().IsSafeForSqlInput())
                    .WithMessage("HeightUnit contains invalid characters.");
            }
        );

        When(
            x => x.WeightUnit is not null,
            () =>
            {
                RuleFor(x => x.WeightUnit)
                    .Must(x => x.HasValue && Enum.IsDefined(typeof(WeightUnit), x))
                    .WithMessage("WeightUnit must be a valid enum value.")
                    .Must(x => x.ToString().IsSafeForSqlInput())
                    .WithMessage("WeightUnit contains invalid characters.");
            }
        );

        When(
            x => x.TeacherId is not null,
            () =>
            {
                RuleFor(x => x.TeacherId!)
                    .Must(x => x.HasValue && x != Guid.Empty)
                    .WithMessage("TeacherId must be a valid Guid when provided.")
                    .Must(x => x.ToString().IsSafeForSqlInput())
                    .WithMessage("Password contains invalid characters.");
            }
        );

        When(
            x => x.FirstName is not null,
            () =>
            {
                RuleFor(x => x.FirstName!)
                    .NotEmpty()
                    .WithMessage("FirstName is required.")
                    .MinimumLength(3)
                    .WithMessage("FirstName must have more than 2 characters.")
                    .MaximumLength(20)
                    .WithMessage("FirstName must be up to 20 characters.")
                    .Matches(RegexPatterns.OnlyLettersPattern())
                    .WithMessage("FirstName contains invalid characters.")
                    .Must(StringHelpers.IsSafeForSqlInput)
                    .WithMessage("FirstName contains invalid characters.");
            }
        );

        When(
            x => x.LastName is not null,
            () =>
            {
                RuleFor(x => x.LastName!)
                    .NotEmpty()
                    .WithMessage("LastName is required.")
                    .MinimumLength(3)
                    .WithMessage("LastName must have more than 2 characters.")
                    .MaximumLength(20)
                    .WithMessage("LastName must be up to 20 characters.")
                    .Matches(RegexPatterns.OnlyLettersPattern())
                    .WithMessage("LastName contains invalid characters.")
                    .Must(StringHelpers.IsSafeForSqlInput)
                    .WithMessage("LastName contains invalid characters.");
            }
        );

        When(
            x => x.Cpf is not null,
            () =>
            {
                RuleFor(x => x.Cpf!)
                    .NotEmpty()
                    .WithMessage("CPF is required.")
                    .Matches(RegexPatterns.CpfPattern())
                    .WithMessage("CPF must be in the format XXX.XXX.XXX-XX.")
                    .Must(StringHelpers.IsValidCpf)
                    .WithMessage("CPF must be valid.")
                    .Must(StringHelpers.IsSafeForSqlInput)
                    .WithMessage("CPF contains invalid characters.");
            }
        );

        When(
            x => x.PhoneNumber is not null,
            () =>
            {
                RuleFor(x => x.PhoneNumber!)
                    .NotEmpty()
                    .WithMessage("PhoneNumber is required.")
                    .Must(StringHelpers.IsValidPhoneNumber)
                    .WithMessage("PhoneNumber must be in the format: +CC (AA) 99999-9999")
                    .Must(StringHelpers.IsSafeForSqlInput)
                    .WithMessage("PhoneNumber contains invalid characters.");
            }
        );

        When(
            x => x.Email is not null,
            () =>
            {
                RuleFor(x => x.Email!)
                    .NotEmpty()
                    .WithMessage("Email is required.")
                    .Must(StringHelpers.IsValidEmail)
                    .WithMessage("Email is not a valid email address.")
                    .Must(StringHelpers.IsSafeForSqlInput)
                    .WithMessage("Email contains invalid characters.");
            }
        );

        When(
            x => x.Height is not null,
            () =>
            {
                RuleFor(x => x.Height)
                    .Must(h =>
                        h is not null
                        && !float.IsNegative(h.Value)
                        && !float.IsNaN(h.Value)
                        && !float.IsInfinity(h.Value)
                    )
                    .WithMessage("Height must be a valid positive number when provided.");
            }
        );

        When(
            x => x.Weight is not null,
            () =>
            {
                RuleFor(x => x.Weight)
                    .Must(w =>
                        w is not null
                        && !float.IsNegative(w.Value)
                        && !float.IsNaN(w.Value)
                        && !float.IsInfinity(w.Value)
                    )
                    .WithMessage("Weight must be a valid positive number when provided.");
            }
        );
    }
}