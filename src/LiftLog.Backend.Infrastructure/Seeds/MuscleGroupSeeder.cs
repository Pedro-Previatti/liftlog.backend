using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LiftLog.Backend.Infrastructure.Seeds;

public class MuscleGroupSeeder
{
    private static readonly List<string> MuscleGroups =
    [
        MuscleGroupsNameConstants.Adductors,
        MuscleGroupsNameConstants.Abductors,
        MuscleGroupsNameConstants.Back,
        MuscleGroupsNameConstants.Biceps,
        MuscleGroupsNameConstants.Calves,
        MuscleGroupsNameConstants.Chest,
        MuscleGroupsNameConstants.Core,
        MuscleGroupsNameConstants.Forearms,
        MuscleGroupsNameConstants.Glutes,
        MuscleGroupsNameConstants.Hamstrings,
        MuscleGroupsNameConstants.Neck,
        MuscleGroupsNameConstants.Quads,
        MuscleGroupsNameConstants.Shoulders,
        MuscleGroupsNameConstants.Traps,
        MuscleGroupsNameConstants.Triceps,
    ];

    public async Task SeedAsync(IServiceProvider provider, CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<LiftLogDatabase>();
        var logger = scope.ServiceProvider.GetService<ILogger<ExerciseSeeder>>();

        var existing = await database
            .MuscleGroups.Select(x => x.Name)
            .ToListAsync(cancellationToken);

        var add = MuscleGroups
            .Except(existing, StringComparer.OrdinalIgnoreCase)
            .Select(MuscleGroup.Create)
            .ToList();

        if (add.Count > 0)
        {
            database.MuscleGroups.AddRange(add);
            await database.SaveChangesAsync(cancellationToken);
            logger?.LogInformation("Seeded {Count} new muscle groups.", add.Count);
        }
        else
        {
            logger?.LogDebug("No new muscle group to seed.");
        }
    }
}
