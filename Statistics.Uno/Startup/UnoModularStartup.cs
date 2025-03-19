using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Statistics.Shared.Startup;

namespace Statistics.Uno.Startup;

public abstract class UnoModularStartup : BaseModularStartup<IUnoStartupModule>, IUnoStartupModule
{

    /// <inheritdoc />
    public virtual void ConfigureApplication(IApplicationBuilder app)
    {
    }

    public IApplicationBuilder SetupApplication(IApplicationBuilder app)
    {
        ConfigureApplication(app);
        foreach (IUnoStartupModule module in _modules)
        {
            module.ConfigureApplication(app);
        }

        return app;
    }
}
