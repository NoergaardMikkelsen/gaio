using Microsoft.EntityFrameworkCore;
using Statistics.Shared.Persistence.Core;
using Statistics.Shared.Persistence.Core.Startup;

namespace Statistics.Uno.Startup;

public class UnoDatabaseContextStartupModule<TContext> : BaseDatabaseContextStartupModule<TContext>, IUnoStartupModule where TContext : BaseDatabaseContext
{
    /// <inheritdoc />
    public UnoDatabaseContextStartupModule(SetupOptionsDelegate setup, bool migrateOnStartup = true) : base(setup, migrateOnStartup)
    {
    }

    public void ConfigureApplication(IApplicationBuilder app)
    {

    }
}
