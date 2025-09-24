using FluentValidation;
using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Core.Validators;

public class ExerciseValidator : AbstractValidator<Exercise>
{
    public ExerciseValidator()
    {
        RuleFor(x => x.MuscleGroupIds)
            .NotNull()
            .WithMessage("MuscleGroupIds is required.")
            .Must(list => list is not null && list.Count > 0 && list.All(g => g != Guid.Empty))
            .WithMessage("MuscleGroupIds must be a valid list of Guids.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MinimumLength(3)
            .WithMessage("Name must have more than 2 characters.")
            .MaximumLength(50)
            .WithMessage("Name must be up to 50 characters.");
    }
}
