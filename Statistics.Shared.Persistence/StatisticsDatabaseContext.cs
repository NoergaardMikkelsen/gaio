using Microsoft.EntityFrameworkCore;
using Statistics.Shared.Abstraction.Enum.Persistence;
using Statistics.Shared.Persistence.Core;

namespace Statistics.Shared.Persistence;

public class StatisticsDatabaseContext : BaseDatabaseContext
{
    public StatisticsDatabaseContext(DbContextOptions options) : base(options)
    {
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var databaseType = DatabaseType.POSTGRESS;

        //modelBuilder.ApplyConfiguration(new Configuration(databaseType));
    }
}
