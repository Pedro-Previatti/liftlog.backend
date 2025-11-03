using FluentValidation;
using LiftLog.Backend.Contracts.Requests.Auth;
using LiftLog.Backend.Core.Helpers;

namespace LiftLog.Backend.Application.Validators.Auth;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required.")
            .Must(StringHelpers.IsSafeForSqlInput)
            .WithMessage("Token contains invalid characters.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("RefreshToken is required.")
            .Must(StringHelpers.IsSafeForSqlInput)
            .WithMessage("RefreshToken contains invalid characters.");
    }
}
