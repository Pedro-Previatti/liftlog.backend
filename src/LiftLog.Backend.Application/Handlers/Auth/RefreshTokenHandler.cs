using LiftLog.Backend.Contracts.Requests.Auth;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Auth;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Interfaces;
using LiftLog.Backend.Core.Interfaces.Repositories;
using MediatR;

namespace LiftLog.Backend.Application.Handlers.Auth;

public class RefreshTokenHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    NotificationContext notificationContext,
    TokenHelper tokenHelper
) : IRequestHandler<RefreshTokenRequest, Response<AuthResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository =
        refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));

    private readonly IUserRepository _userRepository =
        userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private readonly IUnitOfWork _unitOfWork =
        unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    private readonly TokenHelper _tokenHelper =
        tokenHelper ?? throw new ArgumentNullException(nameof(tokenHelper));

    public async Task<Response<AuthResponse>> Handle(
        RefreshTokenRequest request,
        CancellationToken cancellationToken
    )
    {
        var principal = _tokenHelper.GetPrincipalFromExpiredToken(request.RefreshToken);
        if (principal is null)
        {
            _notificationContext.AddNotification(
                Notification.New("Operation", "Invalid refresh token.")
            );
            return Response<AuthResponse>.Failure(_notificationContext.Notifications);
        }
        // var idString = _tokenHelper.GetUserIdFromToken(request.RefreshToken);
        // var id = Guid.Parse(idString);
        // var name = principal.Identity!.Name!;
        var email = _tokenHelper.GetUserEmailFromToken(request.RefreshToken);
        var user = await _userRepository.FindAsync(x => x.Email == email, cancellationToken);
        if (user is null)
        {
            _notificationContext.AddNotification(Notification.New("NotFound", "User not found."));
            return Response<AuthResponse>.Failure(_notificationContext.Notifications);
        }

        var storedRefreshToken = await _refreshTokenRepository.FindAsync(
            x => x.Token == request.RefreshToken && x.UserId == user.Id,
            cancellationToken
        );

        if (storedRefreshToken == null || storedRefreshToken.IsUsed || storedRefreshToken.IsRevoked
        // || storedRefreshToken.Expires < DateTime.UtcNow
        )
        {
            _notificationContext.AddNotification(
                Notification.New("Operation", "Refresh token is used, revoked or does not exists.")
            );
            return Response<AuthResponse>.Failure(_notificationContext.Notifications);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            storedRefreshToken.Use();
            await _refreshTokenRepository.UpdateAsync(storedRefreshToken, cancellationToken);

            var token = _tokenHelper.GenerateJwtToken(user);
            var refreshToken = await _refreshTokenRepository.CreateAsync(
                _tokenHelper.GenerateRefreshToken(user),
                cancellationToken
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Response<AuthResponse>.Success(AuthResponse.New(token, refreshToken.Token));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            _notificationContext.AddNotification(
                Notification.New("Operation", "Refresh Token attempt failed.")
            );
            return Response<AuthResponse>.Failure(_notificationContext.Notifications);
        }
    }
}
