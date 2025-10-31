using LiftLog.Backend.Application;

namespace LiftLog.Backend.Api.ApiConfiguration.Services.Builder;

/// <summary>
/// Provides configuration methods for registering MediatR services in the application.
/// </summary>
public static class MediatrServiceConfiguration
{
    /// <summary>
    /// Adds and configures MediatR services to the application's service collection.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance used to configure services.</param>
    public static void AddMediatr(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(AssemblyScan).Assembly)
        );
    }
}
