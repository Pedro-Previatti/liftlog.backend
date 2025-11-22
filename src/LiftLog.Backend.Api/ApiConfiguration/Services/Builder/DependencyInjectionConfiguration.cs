using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Core.Interfaces;
using LiftLog.Backend.Core.Interfaces.Repositories;
using LiftLog.Backend.Core.Options;
using LiftLog.Backend.Infrastructure;
using LiftLog.Backend.Infrastructure.Persistence;
using LiftLog.Backend.Infrastructure.Repositories;
using LiftLog.Backend.Infrastructure.Seeds;

namespace LiftLog.Backend.Api.ApiConfiguration.Services.Builder;

/// <summary>
/// Provides configuration methods for registering application dependencies in the service container.
/// </summary>
public static class DependencyInjectionConfiguration
{
    /// <summary>
    /// Adds application-specific dependencies and services to the dependency injection container.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance used to configure services.</param>
    public static void AddDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<LiftLogDatabase>();

        builder.Services.AddScoped<NotificationContext>();

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
        builder.Services.AddScoped<IMuscleGroupRepository, MuscleGroupRepository>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        builder.Services.AddScoped<IWorkoutExerciseRepository, WorkoutExerciseRepository>();

        builder.Services.AddTransient<MuscleGroupSeeder>();
        builder.Services.AddTransient<ExerciseSeeder>();

        builder.Services.AddSingleton<BcryptHelper>();
        builder.Services.AddSingleton<TokenHelper>();

        builder
            .Services.AddOptions<BcryptOptions>()
            .Bind(builder.Configuration.GetSection("Bcrypt"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder
            .Services.AddOptions<JwtOptions>()
            .Bind(builder.Configuration.GetSection("Jwt"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
