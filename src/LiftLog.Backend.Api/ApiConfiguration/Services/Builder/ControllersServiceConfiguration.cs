using System.Text.Json;
using System.Text.Json.Serialization;
using LiftLog.Backend.Api.Filters;

namespace LiftLog.Backend.Api.ApiConfiguration.Services.Builder;

/// <summary>
/// Provides configuration methods for setting up controller services in the application.
/// </summary>
public static class ControllersServiceConfiguration
{
    /// <summary>
    /// Adds and configures controller services for the application.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance used to configure services.</param>
    public static void AddControllers(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.MaxDepth = 32;
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .AddMvcOptions(options => options.Filters.Add<NotificationFilter>());
    }
}
