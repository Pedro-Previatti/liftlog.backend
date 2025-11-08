using FluentValidation;
using LiftLog.Backend.Contracts.Requests.Exercises;
using LiftLog.Backend.Core.Enums;
using LiftLog.Backend.Core.Helpers;

namespace LiftLog.Backend.Application.Validators.Exercises.Find;

public class FindExercisesValidator : AbstractValidator<FindExercisesRequest>
{
    public FindExercisesValidator()
    {
        RuleFor(x => x.MuscleGroupParam)
            .Must(mgp => mgp == null || Enum.IsDefined(typeof(MuscleGroupParam), mgp))
            .WithMessage("MuscleGroupParam must be a valid enum value when provided.")
            .Must(mgp => mgp.ToString().IsSafeForSqlInput())
            .WithMessage("MuscleGroupParam contains invalid characters.");
    }
}
