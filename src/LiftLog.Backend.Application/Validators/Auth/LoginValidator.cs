using FluentValidation;
using LiftLog.Backend.Contracts.Requests.Auth;
using LiftLog.Backend.Core.Helpers;

namespace LiftLog.Backend.Application.Validators.Auth;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
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
            .Must(StringHelpers.IsSafeForSqlInput)
            .WithMessage("Password contains invalid characters.");
    }
}
