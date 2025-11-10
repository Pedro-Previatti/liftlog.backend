using FluentValidation;
using LiftLog.Backend.Contracts.Requests.Workouts;
using LiftLog.Backend.Core.Helpers;

namespace LiftLog.Backend.Application.Validators.Workouts.Find;

public class FindWorkoutsValidator : AbstractValidator<FindWorkoutsRequest>
{
    public FindWorkoutsValidator()
    {
        RuleFor(x => x.WorkoutId)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("WorkoutId must be a valid Guid when provided.")
            .Must(id => id.ToString().IsSafeForSqlInput())
            .WithMessage("WorkoutId contains invalid characters.");

        RuleFor(x => x.Search)
            .Must(search => search == null || !string.IsNullOrWhiteSpace(search))
            .WithMessage("If provided, Search needs to be a valid string.")
            .Must(StringHelpers.IsSafeForSqlInput)
            .WithMessage("Search contains invalid characters.");
    }
}
