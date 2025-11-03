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

public class FindSelfHandler(
    IUserRepository userRepository,
    NotificationContext notificationContext,
    IHttpContextAccessor httpContextAccessor
) : IRequestHandler<FindSelfRequest, Response<SelfResponse>>
{
    private readonly IUserRepository _userRepository =
        userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    private readonly IHttpContextAccessor _httpContextAccessor =
        httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public async Task<Response<SelfResponse>> Handle(
        FindSelfRequest request,
        CancellationToken cancellationToken
    )
    {
        var userIdClaim = _httpContextAccessor
            .HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;

        if (!Guid.TryParse(userIdClaim, out var requestingUserId))
        {
            _notificationContext.AddNotification(
                Notification.New("Operation", "Unable to get user Id from request token.")
            );
            return Response<SelfResponse>.Failure(_notificationContext.Notifications);
        }

        var user = await _userRepository.FindAsync(
            u => u.Id == requestingUserId,
            cancellationToken
        );

        if (user is not null)
            return Response<SelfResponse>.Success(SelfResponse.FromEntity(user));

        _notificationContext.AddNotification(
            Notification.New("NotFound", "User not found with provided UserId in request token.")
        );
        return Response<SelfResponse>.Failure(_notificationContext.Notifications);
    }
}
