using LiftLog.Backend.Application.Validators;
using LiftLog.Backend.Application.Validators.Workouts.Find;
using LiftLog.Backend.Contracts.Requests.Workouts;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Workouts;
using LiftLog.Backend.Core.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LiftLog.Backend.Api.Controllers;

/// <summary>
/// Workout Controller
/// </summary>
[Authorize]
[ApiController]
[Route("api/workouts")]
[Consumes("application/json")]
[Produces("application/json")]
public class WorkoutController(ISender sender, NotificationContext notificationContext)
    : ControllerBase
{
    private readonly ISender _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    private readonly RequestValidator _validator = new();

    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    /// <summary>
    /// Handles a query of workouts.
    /// </summary>
    /// <returns>
    /// A response containing a list of workouts data or validation errors
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(NoContent))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(Response<>))]
    public async Task<IActionResult> Find(
        [FromQuery] Guid? workoutId = null,
        [FromQuery] string? search = null
    )
    {
        var request = new FindWorkoutsRequest(workoutId, search);

        if (!_validator.Validate(request, new FindWorkoutsValidator()))
        {
            foreach (var error in _validator.NotificationContext.Notifications)
                _notificationContext.AddNotification(error);
            return UnprocessableEntity(
                Response<List<WorkoutResponse>>.Failure(_notificationContext.Notifications)
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
}
