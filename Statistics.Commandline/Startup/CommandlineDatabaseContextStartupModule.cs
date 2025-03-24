using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
