using LiftLog.Backend.Api.ApiConfiguration.Services.Builder;

namespace LiftLog.Backend.Api.ApiConfiguration;

/// <summary>
/// Provides extension methods to configure all required services
/// during application startup for the LiftLog API.
/// </summary>
public static class BuilderServicesConfiguration
{
    /// <summary>
    /// Registers and configures the core application services used by the LiftLog API.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="WebApplicationBuilder"/> used to configure services and the app's request pipeline.
    /// </param>
    /// <param name="logger">
    /// The <see cref="ILogger"/> instance used to log information during the configuration process.
    /// </param>
    public static void ConfigureBuilderServices(this WebApplicationBuilder builder, ILogger logger)
    {
        builder.AddDependencyInjection();
        logger.LogInformation("Dependency injection successfully configured.");

        builder.AddAuthentication();
        logger.LogInformation("Authentication successfully configured.");

        builder.AddCorsPolicy();
        logger.LogInformation("Cors services successfully configured.");

        builder.AddControllers();
        logger.LogInformation("Controllers successfully configured.");

        builder.AddMediatr();
        logger.LogInformation("MediatR successfully configured.");

        builder.AddDatabase();
        logger.LogInformation("Database successfully configured.");

        builder.AddSwagger();
        logger.LogInformation("Swagger successfully configured.");

        builder.Services.AddHttpContextAccessor();
        logger.LogInformation("Http Context Accessor successfully configured.");

        builder.Services.AddEndpointsApiExplorer();
        logger.LogInformation("Endpoint API explorer successfully configured.");

        builder.Services.AddHealthChecks();
        logger.LogInformation("Health checks successfully configured.");
    }
}
