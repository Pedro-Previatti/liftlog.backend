using System.Security.Claims;
using LiftLog.Backend.Contracts.Requests.Workouts;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Workouts;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Backend.Application.Handlers.Workouts.Find;

public class FindWorkoutsHandler(
    IUserRepository userRepository,
    IWorkoutRepository workoutRepository,
    IWorkoutExerciseRepository workoutExerciseRepository,
    NotificationContext notificationContext,
    IHttpContextAccessor httpContextAccessor
) : IRequestHandler<FindWorkoutsRequest, Response<List<WorkoutResponse>>>
{
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

    public async Task<Response<List<WorkoutResponse>>> Handle(
        FindWorkoutsRequest request,
        CancellationToken cancellationToken
    )
    {
        List<Workout> workouts;
        List<WorkoutExercise> exercises = [];
        List<WorkoutResponse> workoutsResponse = [];

        var userIdClaim = _httpContextAccessor
            .HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;

        if (!Guid.TryParse(userIdClaim, out var requestingUserId))
        {
            _notificationContext.AddNotification(
                Notification.New("Operation", "Unable to get user Id from request token.")
            );
            return Response<List<WorkoutResponse>>.Failure(_notificationContext.Notifications);
        }

        var requestingUser = await _userRepository.FindAsync(
            u => u.Id == requestingUserId,
            cancellationToken
        );
        if (requestingUser is null)
        {
            _notificationContext.AddNotification(
                Notification.New(
                    "NotFound",
                    "User not found with provided UserId in request token."
                )
            );
            return Response<List<WorkoutResponse>>.Failure(_notificationContext.Notifications);
        }

        if (request.WorkoutId.HasValue)
            workouts = await _workoutRepository.FindMultipleAsync(
                x => x.CreatedForUserIds.Contains(requestingUser.Id) && x.Id == request.WorkoutId,
                cancellationToken
            );
        else if (request.Search is not null)
            workouts = await _workoutRepository.FindMultipleAsync(
                x =>
                    x.CreatedForUserIds.Contains(requestingUser.Id)
                    && EF.Functions.ILike(
                        EF.Functions.Unaccent(x.Name),
                        EF.Functions.Unaccent($"%{request.Search}%")
                    ),
                cancellationToken
            );
        else
            workouts = await _workoutRepository.FindMultipleAsync(
                x =>
                    x.CreatedForUserIds.Contains(requestingUser.Id)
                    || x.CreatedBy == requestingUser.Id,
                cancellationToken
            );

        if (workouts.Count is 0)
        {
            _notificationContext.AddNotification(
                Notification.New("NotFound", "No workout found with given parameters.")
            );
            return Response<List<WorkoutResponse>>.Failure(_notificationContext.Notifications);
        }

        var ids = workouts
            .SelectMany(w => w.WorkoutExerciseIds)
            .Distinct()
            .ToList();

        var workoutExercises = await _workoutExerciseRepository.FindMultipleAsync(
            x => ids.Contains(x.Id),
            cancellationToken
        );

        exercises.AddRange(workoutExercises);

        foreach (var workout in workouts)
        {
            List<WorkoutExerciseResponse> matches = [];

            matches.AddRange(
                exercises
                    .Where(e => workout.WorkoutExerciseIds.Contains(e.Id))
                    .Select(WorkoutExerciseResponse.FromEntity)
            );

            workoutsResponse.Add(WorkoutResponse.FromEntities(workout, matches));
        }

        return Response<List<WorkoutResponse>>.Success(workoutsResponse);
    }
}
