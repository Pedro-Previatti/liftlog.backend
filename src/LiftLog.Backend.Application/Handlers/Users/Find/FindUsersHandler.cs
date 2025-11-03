using System.Security.Claims;
using LiftLog.Backend.Contracts.Requests.Users;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Users;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LiftLog.Backend.Application.Handlers.Users.Find;

public class FindUsersHandler(
    IUserRepository userRepository,
    NotificationContext notificationContext,
    IHttpContextAccessor httpContextAccessor
) : IRequestHandler<FindUsersRequest, Response<List<UserResponse>>>
{
    private readonly IUserRepository _userRepository =
        userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    private readonly IHttpContextAccessor _httpContextAccessor =
        httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public async Task<Response<List<UserResponse>>> Handle(
        FindUsersRequest request,
        CancellationToken cancellationToken
    )
    {
        List<User> response;

        var userIdClaim = _httpContextAccessor
            .HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;

        if (!Guid.TryParse(userIdClaim, out var requestingUserId))
        {
            _notificationContext.AddNotification(
                Notification.New("Operation", "Unable to get user Id from request token.")
            );
            return Response<List<UserResponse>>.Failure(_notificationContext.Notifications);
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
            return Response<List<UserResponse>>.Failure(_notificationContext.Notifications);
        }

        if (request.Id.HasValue)
            response = await _userRepository.FindMultipleAsync(
                x => x.Id != requestingUser.Id && x.Id == request.Id,
                cancellationToken
            );
        else if (request.Search != null)
            response = await _userRepository.FindBySearchAsync(request.Search, cancellationToken);
        else
            response = await _userRepository.FindMultipleAsync(
                x => x.Id != requestingUser.Id,
                cancellationToken
            );

        if (response.Count > 0)
            return Response<List<UserResponse>>.Success(
                response.Select(u => UserResponse.FromEntity(u)).OrderBy(x => x.Name).ToList()
            );

        _notificationContext.AddNotification(
            Notification.New("NotFound", "No user found with given parameters.")
        );
        return Response<List<UserResponse>>.Failure(_notificationContext.Notifications);
    }
}
