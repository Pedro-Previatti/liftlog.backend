using FluentValidation;
using LiftLog.Backend.Contracts.Requests.Auth;
using LiftLog.Backend.Core.Enums;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Shared;

namespace LiftLog.Backend.Application.Validators.Auth;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Type)
            .NotNull()
            .WithMessage("Type is required.")
            .Must(t => Enum.IsDefined(typeof(UserType), t))
            .WithMessage("Type must be a valid enum value.")
            .Must(t => t.ToString().IsSafeForSqlInput())
            .WithMessage("Type contains invalid characters.");

        RuleFor(x => x.FirstName)
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

        RuleFor(x => x.LastName)
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

        RuleFor(x => x.Cpf)
            .NotEmpty()
            .WithMessage("CPF is required.")
            .Matches(RegexPatterns.CpfPattern())
            .WithMessage("CPF must be in the format XXX.XXX.XXX-XX.")
            .Must(StringHelpers.IsValidCpf)
            .WithMessage("CPF must be valid.")
            .Must(StringHelpers.IsSafeForSqlInput)
            .WithMessage("CPF contains invalid characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("PhoneNumber is required.")
            .Must(StringHelpers.IsValidPhoneNumber)
            .WithMessage("PhoneNumber must be in the format: +CC (AA) 99999-9999")
            .Must(StringHelpers.IsSafeForSqlInput)
            .WithMessage("PhoneNumber contains invalid characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Must(StringHelpers.IsValidEmail)
            .WithMessage("Email is not a valid email address.")
            .Must(StringHelpers.IsSafeForSqlInput)
            .WithMessage("Email contains invalid characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .Matches(RegexPatterns.PasswordPattern())
            .WithMessage(
                "Password must be at least 12 characters long, have one uppercase letter, one lowercase letter and one digit, one special character."
            )
            .MaximumLength(20)
            .WithMessage("Password must be up to 20 characters.")
            .Must(StringHelpers.IsSafeForSqlInput)
            .WithMessage("Password contains invalid characters.");

        RuleFor(x => x.TeacherId)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("TeacherId must be a valid Guid when provided.")
            .Must(id => id.ToString().IsSafeForSqlInput())
            .WithMessage("Password contains invalid characters.");
    }
}
