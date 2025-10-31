using LiftLog.Backend.Api.Middleware;

namespace LiftLog.Backend.Api.ApiConfiguration.Services.Application;

/// <summary>
/// Provides configuration for setting up the global exception handling middleware.
/// </summary>
public static class GlobalExceptionHandlerServiceConfiguration
{
    /// <summary>
    /// Registers the <see cref="GlobalExceptionHandlerMiddleware"/> in the application's request pipeline.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> used to configure the application's middleware.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance for chaining.</returns>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
