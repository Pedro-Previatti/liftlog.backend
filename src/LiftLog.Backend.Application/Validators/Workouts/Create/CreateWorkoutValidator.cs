using FluentValidation;
using LiftLog.Backend.Contracts.Requests.Workouts;
using LiftLog.Backend.Core.Enums;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Shared;

namespace LiftLog.Backend.Application.Validators.Workouts.Create;

public class CreateWorkoutValidator : AbstractValidator<CreateWorkoutRequest>
{
    public CreateWorkoutValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("FirstName is required.")
            .MaximumLength(20)
            .WithMessage("FirstName must be up to 20 characters.")
            .Matches(RegexPatterns.OnlyLettersPattern())
            .WithMessage("FirstName contains invalid characters.")
            .Must(StringHelpers.IsSafeForSqlInput)
            .WithMessage("FirstName contains invalid characters.");

        RuleForEach(x => x.Exercises).NotEmpty().WithMessage("Exercises are required.");

        RuleForEach(x => x.Exercises)
            .ChildRules(exercise =>
            {
                exercise
                    .RuleFor(e => e.Id)
                    .Must(id => id != Guid.Empty)
                    .WithMessage("Exercise Id must be a valid Guid.");

                exercise
                    .RuleFor(e => e.Sets)
                    .GreaterThan(0)
                    .WithMessage("Sets must be a positive integer.");

                exercise
                    .RuleFor(e => e.Reps)
                    .GreaterThan(0)
                    .WithMessage("Reps must be a positive integer.");

                exercise
                    .RuleFor(e => e.Weight)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Weight must be a positive number.");

                exercise
                    .RuleFor(e => e.Unit)
                    .Must(unit => Enum.IsDefined(typeof(WeightUnit), unit))
                    .WithMessage("WeightUnit must be a valid enum value.");
            });
    }
}
