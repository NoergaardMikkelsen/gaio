using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Statistics.Shared.Startup;

namespace Statistics.Commandline.Startup;

public abstract class CommandlineModularStartup : BaseModularStartup<ICommandlineStartupModule>,
    ICommandlineStartupModule
{
    public IConfiguration Configuration { get; protected set; }

    /// <inheritdoc />
    public virtual void ConfigureApplication(IHostApplicationBuilder app)
    {
    }

    public IHostApplicationBuilder SetupApplication(IHostApplicationBuilder app)
    {
        ConfigureApplication(app);
        foreach (ICommandlineStartupModule module in _modules)
        {
            module.ConfigureApplication(app);
        }

        return app;
    }
}
