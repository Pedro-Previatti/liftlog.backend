using LiftLog.Backend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace LiftLog.Backend.Infrastructure.Conventions
{
    public sealed class BaseEntityConvention : IModelFinalizingConvention
    {
        private const string IdPropertyName = nameof(BaseEntity.Id);
        private const string CreatedAtPropertyName = nameof(BaseEntity.CreatedAtUtc);
        private const string UpdatedAtPropertyName = nameof(BaseEntity.UpdatedAtUtc);

        public void ProcessModelFinalizing(
            IConventionModelBuilder builder,
            IConventionContext<IConventionModelBuilder> context
        )
        {
            var model = builder.Metadata;

            var baseEntityType = typeof(BaseEntity);

            foreach (
                var entityType in model
                    .GetEntityTypes()
                    .Where(t => baseEntityType.IsAssignableFrom(t.ClrType))
            )
            {
                var idProp = entityType.FindProperty(IdPropertyName);
                if (idProp is not null)
                {
                    entityType.Builder.HasKey([idProp]);
                    idProp.Builder.HasColumnType("uuid");
                    idProp.Builder.IsRequired(true);
                }

                var createdProp = entityType.FindProperty(CreatedAtPropertyName);
                if (createdProp is not null)
                {
                    createdProp.Builder.HasColumnType("timestamp with time zone");
                    createdProp.Builder.IsRequired(true);
                }

                var updatedProp = entityType.FindProperty(UpdatedAtPropertyName);
                if (updatedProp is not null)
                {
                    updatedProp.Builder.HasColumnType("timestamp with time zone");
                    updatedProp.Builder.IsRequired(true);
                }

                entityType.Builder.Ignore(nameof(BaseEntity.ValidationResult));
                entityType.Builder.Ignore(nameof(BaseEntity.Valid));
                entityType.Builder.Ignore(nameof(BaseEntity.Invalid));
            }
        }
    }
}
