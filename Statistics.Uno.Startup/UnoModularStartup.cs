using Microsoft.Extensions.DependencyInjection;

namespace Statistics.Uno.Startup;

public abstract class UnoModularStartup : IUnoStartupModule
{
    public IServiceCollection Services { get; private set; }
    public IServiceProvider ServiceProvider { get; private set; }

    private ICollection<IUnoStartupModule> _modules;


    protected UnoModularStartup()
    {
        _modules = new List<IUnoStartupModule>();
    }

    protected void AddModule(IUnoStartupModule module)
    {
        _modules.Add(module);
    }

    /// <inheritdoc />
    public virtual void ConfigureServices(IServiceCollection services)
    {
    }

    /// <inheritdoc />
    public virtual void ConfigureApplication(IApplicationBuilder app)
    {
    }

    public void SetupServices(IServiceCollection? services = null)
    {
        Services = services ??= new ServiceCollection();

        ConfigureServices(services);
        foreach (IUnoStartupModule module in _modules)
        {
            module.ConfigureServices(Services);
        }

        ServiceProvider = Services.BuildServiceProvider();
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
