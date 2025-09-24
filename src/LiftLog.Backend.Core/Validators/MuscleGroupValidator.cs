using FluentValidation;
using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Core.Validators;

public class MuscleGroupValidator : AbstractValidator<MuscleGroup>
{
    public MuscleGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MinimumLength(3)
            .WithMessage("Name must have more than 2 characters.")
            .MaximumLength(30)
            .WithMessage("Name must be up to 30 characters.");
    }
}
