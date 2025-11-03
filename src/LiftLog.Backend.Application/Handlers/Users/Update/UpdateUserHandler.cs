using System.Security.Claims;
using LiftLog.Backend.Contracts.Requests.Users;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Users;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Interfaces;
using LiftLog.Backend.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LiftLog.Backend.Application.Handlers.Users.Update;

public class UpdateUserHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    NotificationContext notificationContext,
    IHttpContextAccessor httpContextAccessor
) : IRequestHandler<UpdateUserRequest, Response<UserResponse>>
{
    private readonly IUserRepository _userRepository =
        userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private readonly IUnitOfWork _unitOfWork =
        unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    private readonly IHttpContextAccessor _httpContextAccessor =
        httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public async Task<Response<UserResponse>> Handle(
        UpdateUserRequest request,
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
            return Response<UserResponse>.Failure(_notificationContext.Notifications);
        }

        var user = await _userRepository.FindAsync(
            u => u.Id == userId,
            cancellationToken
        );
        if (user is null)
        {
            _notificationContext.AddNotification(
                Notification.New(
                    "NotFound",
                    "User not found with provided UserId in request token."
                )
            );
            return Response<UserResponse>.Failure(_notificationContext.Notifications);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var update = _userRepository.UpdateAsync(
                user.Update(
                    request.Type,
                    request.Gender,
                    request.HeightUnit,
                    request.WeightUnit,
                    request.TeacherId,
                    request.FirstName,
                    request.LastName,
                    request.Cpf,
                    request.PhoneNumber,
                    request.Email,
                    null,
                    request.Height,
                    request.Weight
                ),
                cancellationToken
            );

            var result = update.Result;

            if (user.Invalid)
            {
                _notificationContext.AddNotifications(user.GetNotifications());
                return Response<UserResponse>.Failure(_notificationContext.Notifications);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Response<UserResponse>.Success(
                UserResponse.FromEntity(result)
            );
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            _notificationContext.AddNotification(
                Notification.New("Operation", "User updating attempt failed.")
            );
            return Response<UserResponse>.Failure(_notificationContext.Notifications);
        }
    }
}