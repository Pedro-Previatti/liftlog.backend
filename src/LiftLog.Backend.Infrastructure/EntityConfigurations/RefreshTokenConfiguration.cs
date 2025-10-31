using LiftLog.Backend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiftLog.Backend.Infrastructure.EntityConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid").ValueGeneratedNever();

        builder.Property(x => x.UserId).HasColumnType("uuid").IsRequired();

        builder.Property(x => x.Expires).HasColumnType("timestamp with time zone").IsRequired();

        builder.Property(x => x.Token).HasColumnType("varchar(100)").HasMaxLength(100).IsRequired();

        builder.Property(x => x.IsRevoked).HasColumnType("boolean").IsRequired();

        builder.Property(x => x.IsUsed).HasColumnType("boolean").IsRequired();
    }
}
