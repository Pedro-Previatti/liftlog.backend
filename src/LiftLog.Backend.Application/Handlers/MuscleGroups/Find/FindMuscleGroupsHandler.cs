using LiftLog.Backend.Contracts.Requests.MuscleGroups;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.MuscleGroups;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Backend.Application.Handlers.MuscleGroups.Find;

public class FindMuscleGroupsHandler(
    LiftLogDatabase database,
    NotificationContext notificationContext
) : IRequestHandler<FindMuscleGroupsRequest, Response<List<MuscleGroupResponse>>>
{
    private readonly LiftLogDatabase _database =
        database ?? throw new ArgumentNullException(nameof(database));

    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    public async Task<Response<List<MuscleGroupResponse>>> Handle(
        FindMuscleGroupsRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await _database
            .MuscleGroups.OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        if (response.Count > 0)
            return Response<List<MuscleGroupResponse>>.Success(
                response
                    .Select(x => MuscleGroupResponse.FromEntity(x))
                    .OrderBy(x => x.Name)
                    .ToList()
            );

        _notificationContext.AddNotification(
            Notification.New("Operation", "Error during query execution.")
        );
        return Response<List<MuscleGroupResponse>>.Failure(_notificationContext.Notifications);
    }
}
