using LiftLog.Backend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiftLog.Backend.Infrastructure.EntityConfigurations;

public class MuscleGroupConfiguration : IEntityTypeConfiguration<MuscleGroup>
{
    public void Configure(EntityTypeBuilder<MuscleGroup> builder)
    {
        builder.ToTable("MuscleGroup");

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(30)").HasMaxLength(30);

        builder.Ignore(x => x.CreatedBy);
        builder.Ignore(x => x.UpdatedBy);
    }
}
