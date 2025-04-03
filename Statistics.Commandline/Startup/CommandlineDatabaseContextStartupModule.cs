using Microsoft.Extensions.Hosting;
using Statistics.Shared.Persistence.Core;
using Statistics.Shared.Persistence.Core.Startup;

namespace Statistics.Commandline.Startup;

public class CommandlineDatabaseContextStartupModule<TContext> : BaseDatabaseContextStartupModule<TContext>,
    ICommandlineStartupModule where TContext : BaseDatabaseContext
{
    /// <inheritdoc />
    public CommandlineDatabaseContextStartupModule(SetupOptionsDelegate setup, bool migrateOnStartup = true) : base(
        setup, false)
    {
    }

    /// <inheritdoc />
    public void ConfigureApplication(IHostApplicationBuilder app)
    {
    }
}
