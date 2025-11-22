using LiftLog.Backend.Application.Validators;
using LiftLog.Backend.Application.Validators.Users.Find;
using LiftLog.Backend.Application.Validators.Users.Update;
using LiftLog.Backend.Contracts.Requests.Users;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Users;
using LiftLog.Backend.Core.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LiftLog.Backend.Api.Controllers;

/// <summary>
/// User Controller
/// </summary>
[Authorize]
[ApiController]
[Route("api/users")]
[Consumes("application/json")]
[Produces("application/json")]
public class UserController(ISender sender, NotificationContext notificationContext)
    : ControllerBase
{
    private readonly ISender _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    private readonly RequestValidator _validator = new();

    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    /// <summary>
    /// Handles a query of users.
    /// </summary>
    /// <returns>
    /// A response containing a list of users data or validation errors.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<List<UserResponse>>))]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(NoContent))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(Response<>))]
    public async Task<IActionResult> Find(
        [FromQuery] Guid? id = null,
        [FromQuery] string? search = null
    )
    {
        var request = new FindUsersRequest(id, search);

        if (!_validator.Validate(request, new FindUsersValidator()))
        {
            foreach (var error in _validator.NotificationContext.Notifications)
                _notificationContext.AddNotification(error);
            return UnprocessableEntity(
                Response<List<UserResponse>>.Failure(_notificationContext.Notifications)
            );
        }

        var response = await _sender.Send(request);

        if (!response.Successful)
            return response.Errors?.Any(e => e.Key.Equals("NotFound")) == true
                ? NotFound()
                : BadRequest(response);

        var data = response.Data ?? [];

        return data.Count > 0 ? Ok(data) : NoContent();
    }

    /// <summary>
    /// Handles a query of user's own information.
    /// </summary>
    /// <returns>
    /// A response containing a user's own data or validation errors.
    /// </returns>
    [HttpGet("self")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<List<UserResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    public async Task<IActionResult> FindSelf()
    {
        var response = await _sender.Send(new FindSelfRequest());

        if (!response.Successful)
            return response.Errors?.Any(e => e.Key.Equals("NotFound")) == true
                ? NotFound()
                : BadRequest(response);

        return Ok(response.Data);
    }

    /// <summary>
    /// Handles a request to update a user
    /// </summary>
    /// <returns>
    /// A response containing a user's own data or validation errors.
    /// </returns>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<UserResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
    {
        if (!_validator.Validate(request, new UpdateUserValidator()))
        {
            foreach (var error in _validator.NotificationContext.Notifications)
                _notificationContext.AddNotification(error);
            return UnprocessableEntity(
                Response<List<UserResponse>>.Failure(_notificationContext.Notifications)
            );
        }

        var response = await _sender.Send(request);

        if (!response.Successful)
            return response.Errors?.Any(e => e.Key.Equals("NotFound")) == true
                ? NotFound()
                : BadRequest(response);

        return response.Data != null ? Ok(response.Data) : NoContent();
    }
}
