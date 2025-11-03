using LiftLog.Backend.Contracts.Requests.Auth;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Auth;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Interfaces;
using LiftLog.Backend.Core.Interfaces.Repositories;
using MediatR;

namespace LiftLog.Backend.Application.Handlers.Auth;

public class RegisterHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    NotificationContext notificationContext,
    BcryptHelper bcryptHelper,
    TokenHelper tokenHelper
) : IRequestHandler<RegisterRequest, Response<AuthResponse>>
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
        RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        if (
            await _userRepository.HasAsync(x => x.Email == request.Email, cancellationToken)
            || await _userRepository.HasAsync(x => x.Cpf == request.Cpf, cancellationToken)
        )
        {
            _notificationContext.AddNotification(
                Notification.New("Operation", "A user already exists with given email or cpf.")
            );
            return Response<AuthResponse>.Failure(_notificationContext.Notifications);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var create = _userRepository.CreateAsync(
                User.Create(
                    request.Type,
                    request.TeacherId,
                    request.FirstName,
                    request.LastName,
                    request.Cpf,
                    request.PhoneNumber,
                    request.Email,
                    _bcryptHelper.Encrypt(request.Password)
                ),
                cancellationToken
            );

            var user = create.Result;

            if (user.Invalid)
            {
                _notificationContext.AddNotifications(user.GetNotifications());
                return Response<AuthResponse>.Failure(_notificationContext.Notifications);
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
                Notification.New("Operation", "Registration attempt failed.")
            );
            return Response<AuthResponse>.Failure(_notificationContext.Notifications);
        }
    }
}
