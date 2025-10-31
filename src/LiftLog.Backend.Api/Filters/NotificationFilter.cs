using System.Net;
using LiftLog.Backend.Core.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace LiftLog.Backend.Api.Filters;

/// <summary>
/// An action filter that intercepts requests to check for notifications in the <see cref="NotificationContext"/>.
/// If notifications exist, the filter returns a <c>400 Bad Request</c> response containing the notification details.
/// </summary>
public class NotificationFilter(NotificationContext notificationContext) : IAsyncActionFilter
{
    private readonly NotificationContext _notificationContext =
        notificationContext ?? throw new ArgumentNullException(nameof(notificationContext));

    /// <summary>
    /// Executes before and after an action method is invoked to handle notifications in the request context.
    /// </summary>
    /// <param name="context">The current <see cref="ActionExecutingContext"/> containing request and response data.</param>
    /// <param name="next">A delegate to execute the next action filter or action method.</param>
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        if (_notificationContext.HasNotifications)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.HttpContext.Response.ContentType = "application/json";

            var notifications = JsonConvert.SerializeObject(_notificationContext.Notifications);
            await context.HttpContext.Response.WriteAsync(notifications);

            return;
        }
        await next();
    }
}
