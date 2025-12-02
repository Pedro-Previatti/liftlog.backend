using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Workouts;
using LiftLog.Backend.Core.Enums;
using MediatR;

namespace LiftLog.Backend.Contracts.Requests.Workouts;

public record CreateWorkoutRequest(
    string Name,
    DayOfWeek DayOfWeek,
    List<WorkoutExerciseRequest> Exercises
) : IRequest<Response<WorkoutResponse>>;

public record WorkoutExerciseRequest(Guid Id, int Sets, int Reps, float Weight, WeightUnit Unit);
