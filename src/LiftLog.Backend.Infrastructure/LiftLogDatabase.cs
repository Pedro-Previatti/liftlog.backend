using FluentValidation.Results;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Helpers;
using LiftLog.Backend.Infrastructure.Conventions;
using LiftLog.Backend.Infrastructure.Seeds;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Backend.Infrastructure;

public class LiftLogDatabase : DbContext
{
    public LiftLogDatabase(DbContextOptions<LiftLogDatabase> options)
        : base(options) { }

    public DbSet<Exercise> Exercises => Set<Exercise>();

    public DbSet<ExerciseHistory> ExerciseHistory => Set<ExerciseHistory>();

    public DbSet<MuscleGroup> MuscleGroups => Set<MuscleGroup>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Workout> Workouts => Set<Workout>();

    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();

    public DbSet<WorkoutHistory> WorkoutHistory => Set<WorkoutHistory>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Ignore<ValidationFailure>();
        builder.Ignore<ValidationResult>();
        builder.Ignore<Notification>();

        builder.ApplyConfigurationsFromAssembly(typeof(LiftLogDatabase).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        base.ConfigureConventions(builder);

        builder.Conventions.Add(_ => new BaseEntityConvention());
        builder.Conventions.Add(_ => new EnumMappedToStringConvention());
        builder.Conventions.Add(_ => new GuidListMappedToStringConvention());
    }
}
