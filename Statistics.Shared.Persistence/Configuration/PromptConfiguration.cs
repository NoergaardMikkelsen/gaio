using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Statistics.Shared.Abstraction.Enum.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Persistence.Core;

namespace Statistics.Shared.Persistence.Configuration;

public class PromptConfiguration : EntityConfiguration<Prompt>
{
    /// <inheritdoc />
    public PromptConfiguration(DatabaseType databaseType) : base(databaseType)
    {
    }

    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<Prompt> builder)
    {
        base.Configure(builder);
    }
}
