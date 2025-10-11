using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace LiftLog.Backend.Infrastructure.Conventions;

public sealed class EnumMappedToStringConvention : IModelFinalizingConvention
{
    private const int MaxLength = 50;

    public void ProcessModelFinalizing(
        IConventionModelBuilder builder,
        IConventionContext<IConventionModelBuilder> context
    )
    {
        foreach (
            var property in builder
                .Metadata.GetEntityTypes()
                .SelectMany(type =>
                    type.GetDeclaredProperties()
                        .Where(p =>
                            p.ClrType.IsEnum
                            || (Nullable.GetUnderlyingType(p.ClrType)?.IsEnum ?? false)
                        )
                )
        )
        {
            property.Builder.HasConversion(typeof(string));
            property.Builder.HasMaxLength(MaxLength);
        }
    }
}
