namespace LiftLog.Backend.Api.ApiConfiguration.Services.Builder;

/// <summary>
/// Provides configuration methods for setting up CORS policies in the application.
/// </summary>
public static class CorsServiceConfiguration
{
    /// <summary>
    /// The name of the development CORS policy.
    /// </summary>
    public const string DevCorsPolicyName = "DevCorsPolicy";

    /// <summary>
    /// Adds and configures the CORS policy for the application.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance used to configure services.</param>
    public static void AddCorsPolicy(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                name: DevCorsPolicyName,
                configurePolicy: policyBuilder =>
                {
                    policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }
            );
        });
    }
}
