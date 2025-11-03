using FluentValidation;
using LiftLog.Backend.Contracts.Requests.Users;
using LiftLog.Backend.Core.Helpers;

namespace LiftLog.Backend.Application.Validators.Users.Find;

public class FindUsersValidator : AbstractValidator<FindUsersRequest>
{
    public FindUsersValidator()
    {
        RuleFor(x => x.Id)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("UserId must be a valid Guid when provided.")
            .Must(id => id.ToString().IsSafeForSqlInput())
            .WithMessage("UserId contains invalid characters.");

        RuleFor(x => x.Search)
            .Must(search => search == null || !string.IsNullOrWhiteSpace(search))
            .WithMessage("If provided, Search needs to be a valid string.")
            .Must(StringHelpers.IsSafeForSqlInput)
            .WithMessage("Search contains invalid characters.");
    }
}
