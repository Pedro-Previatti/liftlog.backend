using System.Security.Claims;
using LiftLog.Backend.Contracts.Requests.Workouts;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Interfaces;
using LiftLog.Backend.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LiftLog.Backend.Application.Handlers.Workouts.Delete;

public class DeleteWorkoutHandler(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IWorkoutRepository workoutRepository,
    IWorkoutExerciseRepository workoutExerciseRepository,
    NotificationContext notificationContext,
    IHttpContextAccessor httpContextAccessor
) : IRequestHandler<DeleteWorkoutRequest, Response<bool>>
{
    private readonly IUnitOfWork _unitOfWork =
        unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    private readonly IUserRepository _userRepository =
        userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private readonly IWorkoutRepository _workoutRepository =
        workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));

    private readonly IWorkoutExerciseRepository _workoutExerciseRepository =
        workoutExerciseRepository
        ?? throw new ArgumentNullException(nameof(workoutExerciseRepository));

    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    private readonly IHttpContextAccessor _httpContextAccessor =
        httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public async Task<Response<bool>> Handle(DeleteWorkoutRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor
            .HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            _notificationContext.AddNotification(
                Notification.New("Operation", "Unable to get user Id from request token.")
            );
            return Response<bool>.Failure(_notificationContext.Notifications);
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
            return Response<bool>.Failure(_notificationContext.Notifications);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var workout = await _workoutRepository.FindAsync(x => x.Id == request.Id, cancellationToken);
            if (workout is null)
            {
                _notificationContext.AddNotification(
                    Notification.New(
                        "NotFound",
                        "Workout not found with provided in request."
                    )
                );
                return Response<bool>.Failure(_notificationContext.Notifications);
            }

            var workoutExercises = await _workoutExerciseRepository.FindMultipleAsync(x => workout.WorkoutExerciseIds.Contains(x.Id),
                cancellationToken);

            if (workoutExercises.Count > 0)
            {
                foreach (var wExercise in workoutExercises)
                {
                    await _workoutExerciseRepository.KillAsync(wExercise.Id, cancellationToken);
                }
            }

            await _workoutRepository.KillAsync(workout.Id, cancellationToken);

            return Response<bool>.Success(true);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            _notificationContext.AddNotification(
                Notification.New("Operation", "Workout deletion attempt failed.")
            );
            return Response<bool>.Failure(_notificationContext.Notifications);
        }
    }
}