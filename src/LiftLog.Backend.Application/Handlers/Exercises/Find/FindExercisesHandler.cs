using LiftLog.Backend.Contracts.Requests.Exercises;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Exercises;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Enums;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Shared;
using LiftLog.Backend.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Backend.Application.Handlers.Exercises.Find;

public class FindExercisesHandler(LiftLogDatabase database, NotificationContext notificationContext)
    : IRequestHandler<FindExercisesRequest, Response<List<ExerciseResponse>>>
{
    private readonly LiftLogDatabase _database =
        database ?? throw new ArgumentNullException(nameof(database));

    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    public async Task<Response<List<ExerciseResponse>>> Handle(
        FindExercisesRequest request,
        CancellationToken cancellationToken
    )
    {
        var muscleGroups = await _database.MuscleGroups.ToListAsync(cancellationToken);

        var mgFilter = MuscleGroupFilter(muscleGroups, request.MuscleGroupParam);

        if (_notificationContext.HasNotifications)
            return Response<List<ExerciseResponse>>.Failure(_notificationContext.Notifications);

        var exercises = await _database.Exercises.ToListAsync(cancellationToken);

        var filtered = mgFilter.HasValue
            ? exercises.Where(x => x.MuscleGroupIds.Contains(mgFilter.Value)).ToList()
            : exercises.ToList();

        return Response<List<ExerciseResponse>>.Success(
            filtered
                .Select(x => ExerciseResponse.FromEntities(x, muscleGroups))
                .OrderBy(x => x.Name)
                .ToList()
        );
    }

    private Guid? MuscleGroupFilter(List<MuscleGroup> muscleGroups, MuscleGroupParam? param = null)
    {
        var queryMg = MapMuscleGroupQuery(param);

        if (string.IsNullOrWhiteSpace(queryMg))
            return null;

        var mg = muscleGroups.FirstOrDefault(group =>
            group.Name.ToLower().Contains(queryMg.ToLower())
        );

        if (mg is not null)
            return mg.Id;

        _notificationContext.AddNotification(
            Notification.New("NotFound", "No muscle group found for provided query parameter")
        );
        return null;
    }

    private string? MapMuscleGroupQuery(MuscleGroupParam? param = null) =>
        param switch
        {
            MuscleGroupParam.Abductors => MuscleGroupsNameConstants.Abductors,
            MuscleGroupParam.Adductors => MuscleGroupsNameConstants.Adductors,
            MuscleGroupParam.Back => MuscleGroupsNameConstants.Back,
            MuscleGroupParam.Biceps => MuscleGroupsNameConstants.Biceps,
            MuscleGroupParam.Calves => MuscleGroupsNameConstants.Calves,
            MuscleGroupParam.Chest => MuscleGroupsNameConstants.Chest,
            MuscleGroupParam.Core => MuscleGroupsNameConstants.Core,
            MuscleGroupParam.Forearms => MuscleGroupsNameConstants.Forearms,
            MuscleGroupParam.Glutes => MuscleGroupsNameConstants.Glutes,
            MuscleGroupParam.Hamstrings => MuscleGroupsNameConstants.Hamstrings,
            MuscleGroupParam.Neck => MuscleGroupsNameConstants.Neck,
            MuscleGroupParam.Quads => MuscleGroupsNameConstants.Quads,
            MuscleGroupParam.Shoulders => MuscleGroupsNameConstants.Shoulders,
            MuscleGroupParam.Traps => MuscleGroupsNameConstants.Traps,
            MuscleGroupParam.Triceps => MuscleGroupsNameConstants.Triceps,
            _ => null,
        };
}
