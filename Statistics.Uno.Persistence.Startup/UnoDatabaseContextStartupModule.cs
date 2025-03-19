using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Statistics.Shared.Persistence.Core;
using Statistics.Shared.Persistence.Core.Startup;
using Statistics.Uno.Startup;

namespace Statistics.Uno.Persistence.Startup;

public class UnoDatabaseContextStartupModule<TContext> : BaseDatabaseContextStartupModule<TContext>, IUnoStartupModule where TContext : BaseDatabaseContext
{
    /// <inheritdoc />
    public UnoDatabaseContextStartupModule(SetupOptionsDelegate setup, bool migrateOnStartup = true) : base(setup, migrateOnStartup)
    {
    }

    public void ConfigureApplication(IApplicationBuilder app)
    {
        if (!migrateOnStartup)
            return;

        using IServiceScope service =
            app.Build().Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var context = service.ServiceProvider.GetService<TContext>();
        context?.Database.Migrate();

        logger?.LogDebug("Completed Configuration of Database Application.");
    }
}
