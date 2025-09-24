using FluentValidation;
using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Core.Validators;

public class WorkoutHistoryValidator : AbstractValidator<WorkoutHistory>
{
    public WorkoutHistoryValidator()
    {
        RuleFor(x => x.CreatedBy)
            .NotNull()
            .WithMessage("CreatedBy is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("CreatedBy must be a valid Guid.");

        RuleFor(x => x.WorkoutExercisesId)
            .NotNull()
            .WithMessage("WorkoutExercisesId is required.")
            .Must(list => list is not null && list.Count > 0 && list.All(g => g != Guid.Empty))
            .WithMessage("WorkoutExercisesId must be a valid list of Guids.");

        RuleFor(x => x.WorkoutId)
            .NotNull()
            .WithMessage("WorkoutId is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("CreatedBy must be a valid Guid.");
    }
}
