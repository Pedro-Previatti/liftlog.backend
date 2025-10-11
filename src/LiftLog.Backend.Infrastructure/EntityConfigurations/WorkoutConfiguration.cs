using LiftLog.Backend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiftLog.Backend.Infrastructure.EntityConfigurations;

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.ToTable("Workouts");

        builder.Property(x => x.CreatedBy).IsRequired().HasColumnType("uuid");

        builder.Property(x => x.UpdatedBy).IsRequired().HasColumnType("uuid");

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(30)").HasMaxLength(30);
    }
}
