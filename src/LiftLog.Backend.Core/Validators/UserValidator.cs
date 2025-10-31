using FluentValidation;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Enums;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Shared;

namespace LiftLog.Backend.Core.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Type)
            .NotNull()
            .WithMessage("Type is required.")
            .Must(t => Enum.IsDefined(typeof(UserType), t))
            .WithMessage("Type must be a valid enum value.");

        RuleFor(x => x.Gender)
            .Must(g => g == null || Enum.IsDefined(typeof(Gender), g))
            .WithMessage("Gender must be a valid enum value when provided.");

        RuleFor(x => x.HeightUnit)
            .Must(hu => hu == null || Enum.IsDefined(typeof(HeightUnit), hu))
            .WithMessage("HeightUnit must be a valid enum value when provided.");

        RuleFor(x => x.WeightUnit)
            .Must(wu => wu == null || Enum.IsDefined(typeof(WeightUnit), wu))
            .WithMessage("WeightUnit must be a valid enum value when provided.");

        RuleFor(x => x.TeacherId)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("TeacherId must be a valid Guid when provided.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("FirstName is required.")
            .MinimumLength(3)
            .WithMessage("FirstName must have more than 2 characters.")
            .MaximumLength(20)
            .WithMessage("FirstName must be up to 20 characters.")
            .Matches(RegexPatterns.OnlyLettersPattern())
            .WithMessage("FirstName contains invalid characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("LastName is required.")
            .MinimumLength(3)
            .WithMessage("LastName must have more than 2 characters.")
            .MaximumLength(20)
            .WithMessage("LastName must be up to 20 characters.")
            .Matches(RegexPatterns.OnlyLettersPattern())
            .WithMessage("LastName contains invalid characters.");

        RuleFor(x => x.Cpf)
            .NotEmpty()
            .WithMessage("CPF is required.")
            .Matches(RegexPatterns.CpfPattern())
            .WithMessage("CPF must be in the format XXX.XXX.XXX-XX.")
            .Must(StringHelpers.IsValidCpf)
            .WithMessage("CPF must be valid.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("PhoneNumber is required.")
            .Must(StringHelpers.IsValidPhoneNumber)
            .WithMessage("PhoneNumber must be in the format: +CC (AA) 99999-9999.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Must(StringHelpers.IsValidEmail)
            .WithMessage("Email is not a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .Must(StringHelpers.IsCryptographedPassword)
            .WithMessage("Password must be stored in hash.");

        RuleFor(x => x.Height)
            .Must(h =>
                h == null
                || (
                    !float.IsNegative(h.Value)
                    && !float.IsNaN(h.Value)
                    && !float.IsInfinity(h.Value)
                )
            )
            .WithMessage("Height must be a valid positive number when provided.");

        RuleFor(x => x.Weight)
            .Must(w =>
                w == null
                || (
                    !float.IsNegative(w.Value)
                    && !float.IsNaN(w.Value)
                    && !float.IsInfinity(w.Value)
                )
            )
            .WithMessage("Weight must be a valid positive number when provided.");
    }
}
