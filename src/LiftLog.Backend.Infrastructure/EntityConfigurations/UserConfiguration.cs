using LiftLog.Backend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiftLog.Backend.Infrastructure.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(x => x.TeacherId).HasColumnType("uuid");

        builder
            .Property(x => x.FirstName)
            .IsRequired()
            .HasColumnType("varchar(25)")
            .HasMaxLength(25);

        builder
            .Property(x => x.LastName)
            .IsRequired()
            .HasColumnType("varchar(25)")
            .HasMaxLength(25);

        builder.Property(x => x.Cpf).IsRequired().HasColumnType("varchar(15)").HasMaxLength(15);

        builder
            .Property(x => x.PhoneNumber)
            .IsRequired()
            .HasColumnType("varchar(15)")
            .HasMaxLength(15);

        builder.Property(x => x.Email).IsRequired().HasColumnType("varchar(100)").HasMaxLength(100);

        builder
            .Property(x => x.Password)
            .IsRequired()
            .HasColumnType("varchar(400)")
            .HasMaxLength(400);

        builder.Property(x => x.Height).HasColumnType("real");

        builder.Property(x => x.Weight).HasColumnType("real");

        builder.Ignore(x => x.CreatedBy);
        builder.Ignore(x => x.UpdatedBy);
    }
}
