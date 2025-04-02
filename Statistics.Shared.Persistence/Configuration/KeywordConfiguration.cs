using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Statistics.Shared.Abstraction.Enum.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Persistence.Core;

namespace Statistics.Shared.Persistence.Configuration;

public class KeywordConfiguration : EntityConfiguration<Keyword>
{
    /// <inheritdoc />
    public KeywordConfiguration(DatabaseType databaseType) : base(databaseType)
    {
    }

    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<Keyword> builder)
    {
        base.Configure(builder);
    }
}
