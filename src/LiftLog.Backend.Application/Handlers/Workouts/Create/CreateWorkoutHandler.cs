using System.Security.Claims;
using LiftLog.Backend.Contracts.Requests.Workouts;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Workouts;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Interfaces;
using LiftLog.Backend.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LiftLog.Backend.Application.Handlers.Workouts.Create;

public class CreateWorkoutHandler(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IExerciseRepository exerciseRepository,
    IWorkoutRepository workoutRepository,
    IWorkoutExerciseRepository workoutExerciseRepository,
    NotificationContext notificationContext,
    IHttpContextAccessor httpContextAccessor
) : IRequestHandler<CreateWorkoutRequest, Response<WorkoutResponse>>
{
    private readonly IUnitOfWork _unitOfWork =
        unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    private readonly IUserRepository _userRepository =
        userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private readonly IExerciseRepository _exerciseRepository =
        exerciseRepository ?? throw new ArgumentNullException(nameof(exerciseRepository));

    private readonly IWorkoutRepository _workoutRepository =
        workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));

    private readonly IWorkoutExerciseRepository _workoutExerciseRepository =
        workoutExerciseRepository
        ?? throw new ArgumentNullException(nameof(workoutExerciseRepository));

    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    private readonly IHttpContextAccessor _httpContextAccessor =
        httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public async Task<Response<WorkoutResponse>> Handle(
        CreateWorkoutRequest request,
        CancellationToken cancellationToken
    )
    {
        var userIdClaim = _httpContextAccessor
            .HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            _notificationContext.AddNotification(
                Notification.New("Operation", "Unable to get user Id from request token.")
            );
            return Response<WorkoutResponse>.Failure(_notificationContext.Notifications);
        }

        var user = await _userRepository.FindAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            _notificationContext.AddNotification(
                Notification.New(
                    "NotFound",
                    "User not found with provided UserId in request token."
                )
            );
            return Response<WorkoutResponse>.Failure(_notificationContext.Notifications);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var exerciseIds = request.Exercises.Select(x => x.Id).ToList();

            var exercises = await _exerciseRepository.FindMultipleAsync(
                x => exerciseIds.Contains(x.Id),
                cancellationToken
            );

            var exerciseIdsFromDb = exercises.Select(e => e.Id).ToList();

            var missing = exerciseIds.Where(id => !exerciseIdsFromDb.Contains(id)).ToList();

            if (missing.Count > 0)
            {
                _notificationContext.AddNotification(
                    Notification.New(
                        "NotFound",
                        $"Some exercises were not found: {string.Join(", ", missing)}"
                    )
                );
                return Response<WorkoutResponse>.Failure(_notificationContext.Notifications);
            }

            var requestLookup = request.Exercises.ToLookup(r => r.Id);

            var workoutExercises = new List<WorkoutExercise>();

            foreach (var ex in exercises)
            {
                var requestsForExercise = requestLookup[ex.Id].ToList();

                if (requestsForExercise.Count == 0)
                {
                    continue;
                }

                foreach (var req in requestsForExercise)
                {
                    workoutExercises.Add(
                        WorkoutExercise.Create(
                            user.Id,
                            ex.Id,
                            ex.Name,
                            req.Sets,
                            req.Reps,
                            req.Weight,
                            req.Unit
                        )
                    );
                }
            }

            foreach (var workoutExercise in workoutExercises)
            {
                await _workoutExerciseRepository.CreateAsync(workoutExercise, cancellationToken);
            }

            var workoutExerciseIds = workoutExercises.Select(x => x.Id).ToList();

            var workout = await _workoutRepository.CreateAsync(
                Workout.Create(workoutExerciseIds, [user.Id], user.Id, request.Name),
                cancellationToken
            );

            var workoutExerciseResponse = workoutExercises
                .Select(WorkoutExerciseResponse.FromEntity)
                .ToList();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Response<WorkoutResponse>.Success(
                WorkoutResponse.FromEntities(workout, workoutExerciseResponse)
            );
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            _notificationContext.AddNotification(
                Notification.New("Operation", "Workout creation attempt failed.")
            );
            return Response<WorkoutResponse>.Failure(_notificationContext.Notifications);
        }
    }
}
