using FluentValidation;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Enums;

namespace LiftLog.Backend.Core.Validators;

public class WorkoutExerciseValidator : AbstractValidator<WorkoutExercise>
{
    public WorkoutExerciseValidator()
    {
        RuleFor(x => x.CreatedBy)
            .NotNull()
            .WithMessage("CreatedBy is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("CreatedBy must be a valid Guid.");

        RuleFor(x => x.UpdatedBy)
            .NotNull()
            .WithMessage("UpdatedBy is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("UpdatedBy must be a valid Guid.");

        RuleFor(x => x.ExerciseId)
            .NotNull()
            .WithMessage("ExerciseId is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("ExerciseId must be a valid Guid.");

        RuleFor(x => x.WeightUnit)
            .Must(wu => Enum.IsDefined(typeof(WeightUnit), wu))
            .WithMessage("WeightUnit must be a valid enum value.");

        RuleFor(x => x.Sets)
            .NotNull()
            .WithMessage("Sets is required.")
            .Must(s => !int.IsNegative(s))
            .WithMessage("Sets must be a valid positive integer.");

        RuleFor(x => x.Reps)
            .NotNull()
            .WithMessage("Reps is required.")
            .Must(r => !int.IsNegative(r))
            .WithMessage("Reps must be a valid positive integer.");

        RuleFor(x => x.Weight)
            .NotNull()
            .WithMessage("Weight is required.")
            .Must(w => !float.IsNegative(w) && !float.IsNaN(w) && !float.IsInfinity(w))
            .WithMessage("Weight must be a valid positive number.");
    }
}
