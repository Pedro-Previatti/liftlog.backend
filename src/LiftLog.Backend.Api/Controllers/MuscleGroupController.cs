using LiftLog.Backend.Contracts.Requests.MuscleGroups;
using LiftLog.Backend.Contracts.Responses;
using LiftLog.Backend.Contracts.Responses.MuscleGroups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LiftLog.Backend.Api.Controllers;

/// <summary>
/// Muscle Group Controller
/// </summary>
[Authorize]
[ApiController]
[Route("/api/musclegroups")]
[Consumes("application/json")]
[Produces("application/json")]
public class MuscleGroupController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    /// <summary>
    /// Handles a query of Muscle Groups
    /// </summary>
    /// <returns>
    /// A response containing a list of muscle groups data or validation errors.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(Response<List<MuscleGroupResponse>>)
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(NoContent))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    public async Task<IActionResult> Find()
    {
        var response = await _sender.Send(new FindMuscleGroupsRequest());

        if (!response.Successful)
            return BadRequest(response);

        var data = response.Data ?? [];

        return data.Count > 0 ? Ok(data) : NoContent();
    }
}
