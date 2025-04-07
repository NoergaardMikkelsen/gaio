using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Statistics.Shared.Abstraction.Enum.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Persistence.Core;

namespace Statistics.Shared.Persistence.Configuration;

public class ArtificialIntelligenceConfiguration : EntityConfiguration<ArtificialIntelligence>
{
    /// <inheritdoc />
    public ArtificialIntelligenceConfiguration(DatabaseType databaseType) : base(databaseType)
    {
    }

    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<ArtificialIntelligence> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.AiType).IsUnique();
        builder.Property(x => x.AiType).IsRequired();
    }
}
