using LiftLog.Backend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiftLog.Backend.Infrastructure.EntityConfigurations;

public class ExerciseHistoryConfiguration : IEntityTypeConfiguration<ExerciseHistory>
{
    public void Configure(EntityTypeBuilder<ExerciseHistory> builder)
    {
        builder.ToTable("ExerciseHistory");

        builder.Property(x => x.CreatedBy).IsRequired().HasColumnType("uuid");

        builder.Property(x => x.ExerciseId).IsRequired().HasColumnType("uuid");

        builder.Property(x => x.Set).IsRequired().HasColumnType("smallint");

        builder.Property(x => x.Reps).IsRequired().HasColumnType("integer");

        builder.Property(x => x.Weight).IsRequired().HasColumnType("real");

        builder.Ignore(x => x.UpdatedBy);
    }
}
