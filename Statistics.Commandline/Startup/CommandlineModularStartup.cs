using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Statistics.Shared.Startup;

namespace Statistics.Commandline.Startup;

public abstract class CommandlineModularStartup : BaseModularStartup<ICommandlineStartupModule>, ICommandlineStartupModule
{
    public IConfiguration Configuration { get; protected set; }

    /// <inheritdoc />
    public virtual void ConfigureApplication(IHostApplicationBuilder app)
    {
        app.Configuration.AddJsonFile("appsettings.secrets.json", false, true);

        Configuration = app.Configuration;
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
