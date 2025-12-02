using FluentValidation;
using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Core.Validators;

public class WorkoutValidator : AbstractValidator<Workout>
{
    public WorkoutValidator()
    {
        RuleFor(x => x.CreatedBy)
            .NotNull()
            .WithMessage("CreatedBy is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("CreatedBy must be a valid Guid.");

        RuleFor(x => x.WorkoutExerciseIds)
            .NotNull()
            .WithMessage("WorkoutExerciseIds is required.")
            .Must(list => list is not null && list.Count > 0 && list.All(g => g != Guid.Empty))
            .WithMessage("WorkoutExerciseIds must be a valid list of Guids.");

        RuleFor(x => x.CreatedForUserIds)
            .NotNull()
            .WithMessage("CreatedForUserIds is required.")
            .Must(list => list is not null && list.Count > 0 && list.All(g => g != Guid.Empty))
            .WithMessage("CreatedForUserIds must be a valid list of Guids.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MinimumLength(3)
            .WithMessage("Name must have more than 2 characters.")
            .MaximumLength(30)
            .WithMessage("Name must be up to 30 characters.");

        RuleFor(x => x.DayOfWeek)
            .NotNull()
            .WithMessage("Day of the week is required.")
            .Must(t => Enum.IsDefined(typeof(DayOfWeek), t))
            .WithMessage("Day of the week must be a valid enum value.");
    }
}
