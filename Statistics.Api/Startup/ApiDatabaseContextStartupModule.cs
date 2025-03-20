using Statistics.Shared.Persistence.Core;
using Statistics.Shared.Persistence.Core.Startup;
using IApplicationBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;

namespace Statistics.Api.Startup;

public class ApiDatabaseContextStartupModule<TContext> : BaseDatabaseContextStartupModule<TContext>, IApiStartupModule
    where TContext : BaseDatabaseContext
{
    /// <inheritdoc />
    public ApiDatabaseContextStartupModule(SetupOptionsDelegate setup, bool migrateOnStartup = true) : base(setup, migrateOnStartup)
    {
    }

    /// <inheritdoc />
    public void ConfigureApplication(IApplicationBuilder app)
    {
    }
}
