using System.Net;
using System.Text.Json;
using FluentValidation;
using LiftLog.Backend.Core.Exceptions;
using LiftLog.Backend.Core.Shared;
using Microsoft.AspNetCore.Mvc;

namespace LiftLog.Backend.Api.Middleware;

/// <summary>
/// Middleware for global treatment of exceptions on the API
/// </summary>
/// <param name="next">Next middleware on the pipeline</param>
/// <param name="logger">Logger to register information about exceptions</param>
public class GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger
)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly bool _isDevelopment =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() == "development";

    /// <summary>
    /// Process HTTP request and treats not captured exceptions
    /// </summary>
    /// <param name="context">HTTP request's context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles exceptions asynchronously
    /// </summary>
    /// <param name="context">HTTP request's context</param>
    /// <param name="exception">Exception to handle</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails { Instance = context.Request.Path };

        switch (exception)
        {
            case ConfigurationException { HasValidationErrors: true } configEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Validation Error";
                problemDetails.Detail = exception.Message;
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

                problemDetails.Extensions["errors"] = configEx.ValidationErrors;
                break;

            case ConfigurationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Configuration Error";
                problemDetails.Detail = exception.Message;
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                break;

            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Validation Error";

                var errors = validationEx
                    .Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => string.IsNullOrEmpty(g.Key) ? "General" : g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                problemDetails.Detail = string.Join(
                    "; ",
                    validationEx.Errors.Select(e => e.ErrorMessage)
                );
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

                problemDetails.Extensions["errors"] = errors;
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Invalid Argument";
                problemDetails.Detail = argEx.Message;
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                problemDetails.Title = "Not Authorized";
                problemDetails.Detail = "You do not have permission to access this resource";
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                break;

            case HttpRequestException { StatusCode: not null } httpEx:
                context.Response.StatusCode = (int)httpEx.StatusCode!.Value;
                problemDetails.Title = "Communication Error";
                problemDetails.Detail = httpEx.Message;
                problemDetails.Status = (int)httpEx.StatusCode.Value;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail =
                    "An unexpected error occurred. Please contact the development team";
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                break;
        }

        if (_isDevelopment)
        {
            problemDetails.Extensions["exception"] = new
            {
                message = exception.Message,
                stackTrace = exception.StackTrace,
                source = exception.Source,
            };
        }

        var json = JsonSerializer.Serialize(problemDetails, JsonDefaults.CamelCaseIndented);

        await context.Response.WriteAsync(json);
    }
}
