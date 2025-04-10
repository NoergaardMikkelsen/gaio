using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Statistics.Shared.Abstraction.Interfaces.Persistence;

namespace Statistics.Shared.Persistence.Core;

public abstract class BaseDatabaseContext : DbContext
{
    private readonly DbContextOptions options;

    protected BaseDatabaseContext([NotNull] DbContextOptions options) : base(options)
    {
        this.options = options;
    }

    public override int SaveChanges()
    {
        UpdateDatetimes();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateDatetimes();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateDatetimes();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        UpdateDatetimes();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void UpdateDatetimes()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is IEntity && e.State is EntityState.Added or EntityState.Modified);

        foreach (EntityEntry entityEntry in entries)
        {
            ((IEntity) entityEntry.Entity).UpdatedDateTime = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                ((IEntity) entityEntry.Entity).CreatedDateTime = DateTime.UtcNow;
            }
        }
    }
}
