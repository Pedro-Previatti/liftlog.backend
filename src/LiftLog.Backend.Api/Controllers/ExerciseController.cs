using LiftLog.Backend.Application.Validators;
using LiftLog.Backend.Application.Validators.Exercises.Find;
using LiftLog.Backend.Contracts.Requests.Exercises;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.Exercises;
using LiftLog.Backend.Core.Enums;
using LiftLog.Backend.Core.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftLog.Backend.Api.Controllers;

/// <summary>
/// Exercise Controller
/// </summary>
[Authorize]
[ApiController]
[Route("/api/exercises")]
[Consumes("application/json")]
[Produces("application/json")]
public class ExerciseController(ISender sender, NotificationContext notificationContext)
    : ControllerBase
{
    private readonly ISender _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    private readonly RequestValidator _validator = new();

    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    /// <summary>
    /// Handles a query of exercises.
    /// </summary>
    /// <param name="muscle">Nullable Muscle Group to filter exercises</param>
    /// <returns>
    /// A response containing a list of exercise data or validation errors.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<List<ExerciseResponse>>))]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(NoContentResult))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
    public async Task<IActionResult> Find([FromQuery] MuscleGroupParam? muscle = null)
    {
        var request = new FindExercisesRequest(muscle);

        if (!_validator.Validate(request, new FindExercisesValidator()))
        {
            foreach (var error in _validator.NotificationContext.Notifications)
                _notificationContext.AddNotification(error);
            return UnprocessableEntity(
                Response<List<ExerciseResponse>>.Failure(_notificationContext.Notifications)
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
