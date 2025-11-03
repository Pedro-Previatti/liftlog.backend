using LiftLog.Backend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Backend.Api.ApiConfiguration.Services.Builder;

/// <summary>
/// Provides configuration for setting up the database context and connection.
/// </summary>
public static class DatabaseServiceConfiguration
{
    /// <summary>
    /// Adds the PostgreSQL database context to the dependency injection container using the connection string
    /// defined in the application configuration.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance used to configure services.</param>
    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("LiftLogDatabase");

        builder.Services.AddDbContext<LiftLogDatabase>(opt =>
        {
            opt.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly("LiftLog.Backend.Infrastructure");
                }
            );
        });
    }
}
