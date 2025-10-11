using LiftLog.Backend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiftLog.Backend.Infrastructure.EntityConfigurations;

public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
    {
        builder.ToTable("WorkoutExercises");

        builder.Property(x => x.CreatedBy).IsRequired().HasColumnType("uuid");

        builder.Property(x => x.UpdatedBy).IsRequired().HasColumnType("uuid");

        builder.Property(x => x.ExerciseId).IsRequired().HasColumnType("uuid");

        builder.Property(x => x.Sets).IsRequired().HasColumnType("smallint");

        builder.Property(x => x.Reps).IsRequired().HasColumnType("integer");

        builder.Property(x => x.Weight).IsRequired().HasColumnType("real");
    }
}
