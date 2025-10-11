using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LiftLog.Backend.Infrastructure.Conventions;

public class GuidListMappedToStringConvention : IModelFinalizingConvention
{
    private const string Separator = ";";

    public void ProcessModelFinalizing(
        IConventionModelBuilder builder,
        IConventionContext<IConventionModelBuilder> context
    )
    {
        foreach (var entityType in builder.Metadata.GetEntityTypes())
        {
            foreach (
                var property in entityType
                    .GetDeclaredProperties()
                    .Where(p => p.ClrType == typeof(List<Guid>))
            )
            {
                var propertyBuilder = property.Builder;

                propertyBuilder.HasColumnName(property.Name);

                propertyBuilder.HasConversion(
                    new ValueConverter<List<Guid>, string>(
                        guids => string.Join(Separator, guids.Select(g => g.ToString())),
                        str =>
                            str.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(Guid.Parse)
                                .ToList()
                    )
                );
                propertyBuilder.HasValueComparer(
                    new ValueComparer<List<Guid>>(
                        (c1, c2) => c1!.SequenceEqual(c2!),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()
                    )
                );
            }
        }
    }
}
