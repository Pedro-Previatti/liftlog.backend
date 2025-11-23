using LiftLog.Backend.Contracts.Requests.Auth;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Auth;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Interfaces;
using LiftLog.Backend.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiftLog.Backend.Application.Handlers.Auth;

public class LoginHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    NotificationContext notificationContext,
    BcryptHelper bcryptHelper,
    TokenHelper tokenHelper
) : IRequestHandler<LoginRequest, Response<AuthResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository =
        refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));

    private readonly IUserRepository _userRepository =
        userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private readonly IUnitOfWork _unitOfWork =
        unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    private readonly BcryptHelper _bcryptHelper =
        bcryptHelper ?? throw new ArgumentNullException(nameof(bcryptHelper));

    private readonly TokenHelper _tokenHelper =
        tokenHelper ?? throw new ArgumentNullException(nameof(tokenHelper));

    public async Task<Response<AuthResponse>> Handle(
        LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.FindAsync(
            x => x.Email == request.Email,
            cancellationToken
        );
        if (user is null || !_bcryptHelper.Verify(request.Password, user.Password))
        {
            _notificationContext.AddNotification(
                Notification.New("NotFound", "Invalid login attempt.")
            );
            return Response<AuthResponse>.Failure(_notificationContext.Notifications);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var existingToken = await _refreshTokenRepository.FindAsync(
                x => x.UserId == user.Id && (!x.IsUsed || !x.IsRevoked),
                cancellationToken
            );
            if (existingToken is not null)
            {
                existingToken.Revoke();
                await _refreshTokenRepository.UpdateAsync(existingToken, cancellationToken);
            }

            var token = _tokenHelper.GenerateJwtToken(user);
            var refreshToken = _refreshTokenRepository.CreateAsync(
                _tokenHelper.GenerateRefreshToken(user),
                cancellationToken
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Response<AuthResponse>.Success(
                AuthResponse.New(token, refreshToken.Result.Token)
            );
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            _notificationContext.AddNotification(
                Notification.New("Operation", "Login attempt failed.")
            );
            return Response<AuthResponse>.Failure(_notificationContext.Notifications);
        }
    }
}
