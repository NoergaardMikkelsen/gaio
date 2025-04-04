using Microsoft.Extensions.Hosting;
using Statistics.Shared.Abstraction.Interfaces.Refit;
using Statistics.Shared.Startup.Modules;

namespace Statistics.Commandline.Startup;

public class CommandlineRefitStartupModule<TEndpoint> : RefitStartupModule<TEndpoint>, ICommandlineStartupModule
    where TEndpoint : IRefitEndpoint
{
    /// <inheritdoc />
    public CommandlineRefitStartupModule(string baseAddress) : base(baseAddress)
    {
    }

    /// <inheritdoc />
    public void ConfigureApplication(IHostApplicationBuilder app)
    {
    }
}
