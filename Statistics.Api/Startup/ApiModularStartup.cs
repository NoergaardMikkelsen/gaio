using Statistics.Shared.Startup;
using IApplicationBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;

namespace Statistics.Api.Startup;

public class ApiModularStartup : BaseModularStartup<IApiStartupModule>, IApiStartupModule
{
    public IConfiguration Configuration { get; protected set; }

    /// <inheritdoc />
    public virtual void ConfigureApplication(IApplicationBuilder app)
    {
    }

    public IApplicationBuilder SetupApplication(IApplicationBuilder app)
    {
        ConfigureApplication(app);
        foreach (IApiStartupModule module in _modules)
        {
            module.ConfigureApplication(app);
        }

        return app;
    }
}
