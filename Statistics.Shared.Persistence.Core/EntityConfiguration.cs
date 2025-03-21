using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Statistics.Shared.Abstraction.Enum.Persistence;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Persistence.Core.Converters;

namespace Statistics.Shared.Persistence.Core;

public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class, IEntity
{
    private readonly DatabaseType databaseType;

    protected EntityConfiguration(DatabaseType databaseType)
    {
        this.databaseType = databaseType;
    }

    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        Type type = typeof(TEntity);
        var idName = $"{type.Name}Id";

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName(idName);

        switch (databaseType)
        {
            case DatabaseType.SQlite:
                builder.Property(x => x.Version).IsRowVersion().HasConversion(new SqliteTimestampConverter())
                    .HasColumnType("BLOB").HasDefaultValueSql("CURRENT_TIMESTAMP");
                break;
            case DatabaseType.MS_SQL:
            case DatabaseType.POSTGRESS:
                builder.Property(x => x.Version).IsRowVersion();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
