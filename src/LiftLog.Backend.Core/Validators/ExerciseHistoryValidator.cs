using FluentValidation;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Enums;

namespace LiftLog.Backend.Core.Validators;

public class ExerciseHistoryValidator : AbstractValidator<ExerciseHistory>
{
    public ExerciseHistoryValidator()
    {
        RuleFor(x => x.CreatedBy)
            .NotNull()
            .WithMessage("CreatedBy is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("CreatedBy must be a valid Guid.");

        RuleFor(x => x.ExerciseId)
            .NotNull()
            .WithMessage("ExerciseId is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("ExerciseId must be a valid Guid.");

        RuleFor(x => x.WeightUnit)
            .NotNull()
            .WithMessage("WeightUnit is required.")
            .Must(wu => Enum.IsDefined(typeof(WeightUnit), wu))
            .WithMessage("WeightUnit must be a valid enum.");

        RuleFor(x => x.Set)
            .NotNull()
            .WithMessage("Set is required.")
            .Must(s => !int.IsNegative(s))
            .WithMessage("Set must be a valid positive integer.");

        RuleFor(x => x.Reps)
            .NotNull()
            .WithMessage("Reps is required.")
            .Must(s => !int.IsNegative(s))
            .WithMessage("Reps must be a valid positive integer.");

        RuleFor(x => x.Weight)
            .NotNull()
            .WithMessage("Weight is required.")
            .Must(s => !float.IsNegative(s) || !float.IsNaN(s) || !float.IsInfinity(s))
            .WithMessage("Weight must be a valid positive number.");
    }
}
