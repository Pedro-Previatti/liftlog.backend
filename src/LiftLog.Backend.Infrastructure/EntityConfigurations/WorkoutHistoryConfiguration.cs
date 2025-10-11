using LiftLog.Backend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiftLog.Backend.Infrastructure.EntityConfigurations;

public class WorkoutHistoryConfiguration : IEntityTypeConfiguration<WorkoutHistory>
{
    public void Configure(EntityTypeBuilder<WorkoutHistory> builder)
    {
        builder.ToTable("WorkoutHistory");

        builder.Property(x => x.CreatedBy).IsRequired().HasColumnType("uuid");

        builder.Property(x => x.WorkoutId).IsRequired().HasColumnType("uuid");

        builder.Ignore(x => x.UpdatedBy);
    }
}
