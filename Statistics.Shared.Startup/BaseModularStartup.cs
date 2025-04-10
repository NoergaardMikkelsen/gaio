using Microsoft.Extensions.DependencyInjection;
using Statistics.Shared.Abstraction.Interfaces.Startup;

namespace Statistics.Shared.Startup;

public abstract class BaseModularStartup<TModule> : IStartupModule where TModule : IStartupModule
{
    protected ICollection<TModule> _modules;

    protected BaseModularStartup()
    {
        _modules = new List<TModule>();
    }

    public IServiceCollection Services { get; protected set; }
    public IServiceProvider ServiceProvider { get; protected set; }

    /// <inheritdoc />
    public virtual void ConfigureServices(IServiceCollection services)
    {
    }

    protected void AddModule(TModule module)
    {
        _modules.Add(module);
    }

    public void SetupServices(IServiceCollection? services = null)
    {
        Services = services ??= new ServiceCollection();

        ConfigureServices(services);
        foreach (TModule module in _modules)
        {
            module.ConfigureServices(Services);
        }


        ServiceProvider = Services.BuildServiceProvider();
    }
}
