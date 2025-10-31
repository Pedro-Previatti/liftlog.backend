using LiftLog.Backend.Api.ApiConfiguration;
using LiftLog.Backend.Api.ApiConfiguration.Services.Application;
using LiftLog.Backend.Api.ApiConfiguration.Services.Builder;
using LiftLog.Backend.Infrastructure;
using LiftLog.Backend.Infrastructure.Seeds;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var loggerFactory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Information).AddConsole());

var logger = loggerFactory.CreateLogger<Program>();

try
{
    builder.ConfigureBuilderServices(logger);

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var provider = scope.ServiceProvider;

        var context = provider.GetRequiredService<LiftLogDatabase>();

        var pending = context.Database.GetPendingMigrations().ToList();

        if (pending.Count > 0)
        {
            foreach (var migration in pending)
            {
                logger.LogInformation("Applying migration: {Migration}", migration);
            }

            context.Database.Migrate();
            logger.LogInformation("Database migrations applied.");
        }
        else
        {
            logger.LogInformation("No migrations to apply.");
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseCors(CorsServiceConfiguration.DevCorsPolicyName);
            logger.LogInformation("Development CORS policy enabled.");

            app.UseSwagger();
            app.UseSwaggerUI();
            logger.LogInformation("Swagger enabled for development environment.");

            var mgSeeder = provider.GetRequiredService<MuscleGroupSeeder>();
            var exSeeder = provider.GetRequiredService<ExerciseSeeder>();

            await mgSeeder.SeedAsync(provider, CancellationToken.None);
            await exSeeder.SeedAsync(provider, CancellationToken.None);
        }
    }

    app.UseGlobalExceptionHandler();
    logger.LogInformation("Global exception handler successfully configured.");

    app.UseHttpsRedirection();
    logger.LogInformation("HTTPS redirection enabled.");

    app.MapHealthChecks("/health");
    app.MapHealthChecks("/health/startup");
    app.MapHealthChecks("/health/liveness", new HealthCheckOptions { Predicate = _ => false });
    logger.LogInformation("Health checks successfully mapped.");

    app.MapControllers();
    logger.LogInformation("Controllers mapped.");

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        foreach (var url in app.Urls)
        {
            logger.LogInformation("Application is running on: {Url}", url);
        }
    });

    logger.LogInformation(
        "Application successfully started in environment: {Environment}",
        app.Environment.EnvironmentName
    );

    app.Run();
}
catch (Exception e)
{
    logger.LogCritical(e, "The application failed to start due to an exception.");
}
finally
{
    logger.LogInformation("Closing application.");
}
