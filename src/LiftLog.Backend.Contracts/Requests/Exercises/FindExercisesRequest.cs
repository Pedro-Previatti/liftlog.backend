using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Exercises;
using LiftLog.Backend.Core.Enums;
using MediatR;

namespace LiftLog.Backend.Contracts.Requests.Exercises;

public record FindExercisesRequest(MuscleGroupParam? MuscleGroupParam = null, string? Search = null)
    : IRequest<Response<List<ExerciseResponse>>>;
