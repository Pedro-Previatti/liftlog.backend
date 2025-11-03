using LiftLog.Backend.Application.Validators;
using LiftLog.Backend.Application.Validators.Auth;
using LiftLog.Backend.Contracts.Requests.Auth;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Auth;
using LiftLog.Backend.Core.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LiftLog.Backend.Api.Controllers;

/// <summary>
/// Controller responsible for user authentication, registration, login, and token refresh.
/// </summary>
[ApiController]
[Route("/api/auth")]
[Consumes("application/json")]
[Produces("application/json")]
public class AuthController(ISender sender, NotificationContext notificationContext)
    : ControllerBase
{
    private readonly ISender _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    private readonly RequestValidator _validator = new();

    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="request">
    /// The registration details including user type, name, CPF, phone, email, password, and optional teacher ID.
    /// </param>
    /// <returns>
    /// A response containing authentication data or validation errors.
    /// </returns>
    [HttpPost("/register")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Response<AuthResponse>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<>))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!_validator.Validate(request, new RegisterValidator()))
        {
            foreach (var error in _validator.NotificationContext.Notifications)
                _notificationContext.AddNotification(error);
            return UnprocessableEntity(
                Response<List<AuthResponse>>.Failure(_notificationContext.Notifications)
            );
        }

        var response = await _sender.Send(request);

        if (!response.Successful)
            return BadRequest(response);

        return Ok(response.Data);
    }

    /// <summary>
    /// Logs in a user with email and password.
    /// </summary>
    /// <param name="request">
    /// The login credentials: email and password.
    /// </param>
    /// <returns>
    /// A response containing authentication data or validation errors.
    /// </returns>
    [HttpPost("/login")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Response<AuthResponse>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!_validator.Validate(request, new LoginValidator()))
        {
            foreach (var error in _validator.NotificationContext.Notifications)
                _notificationContext.AddNotification(error);
            return UnprocessableEntity(
                Response<List<AuthResponse>>.Failure(_notificationContext.Notifications)
            );
        }

        var response = await _sender.Send(request);

        if (!response.Successful)
            return response.Errors?.Any(e => e.Key.Equals("NotFound")) == true
                ? NotFound()
                : BadRequest(response);

        return Ok(response.Data);
    }

    /// <summary>
    /// Refreshes a user's JWT token using a refresh token.
    /// </summary>
    /// <param name="request">
    /// The refresh token request containing the current token and refresh token.
    /// </param>
    /// <returns>
    /// A response containing new authentication data or validation errors.
    /// </returns>
    [HttpPost("/refresh-token")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Response<AuthResponse>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!_validator.Validate(request, new RefreshTokenValidator()))
        {
            foreach (var error in _validator.NotificationContext.Notifications)
                _notificationContext.AddNotification(error);
            return UnprocessableEntity(
                Response<List<AuthResponse>>.Failure(_notificationContext.Notifications)
            );
        }

        var response = await _sender.Send(request);

        if (!response.Successful)
            return response.Errors?.Any(e => e.Key.Equals("NotFound")) == true
                ? NotFound()
                : BadRequest(response);

        return Ok(response.Data);
    }
}
