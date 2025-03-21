using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Statistics.Shared.Abstraction.Enum.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Persistence.Core;

namespace Statistics.Shared.Persistence.Configuration;

public class ResponseConfiguration : EntityConfiguration<Response>
{
    /// <inheritdoc />
    public ResponseConfiguration(DatabaseType databaseType) : base(databaseType)
    {
    }

    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<Response> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => (Prompt) x.Prompt).WithMany(x => (ICollection<Response>) x.Responses)
            .HasForeignKey(x => x.PromptId);
        builder.HasOne(x => (ArtificialIntelligence) x.Ai).WithMany(x => (ICollection<Response>) x.Responses)
            .HasForeignKey(x => x.AiId);
    }
}
