using FluentValidation;
using LiftLog.Backend.Contracts.Requests.Workouts;

namespace LiftLog.Backend.Application.Validators.Workouts.Delete;

public class DeleteWorkoutValidator : AbstractValidator<DeleteWorkoutRequest>
{
    public DeleteWorkoutValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Workout id is required");
    }
}