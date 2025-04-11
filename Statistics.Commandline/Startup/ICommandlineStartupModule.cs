using Microsoft.Extensions.Hosting;
using Statistics.Shared.Abstraction.Interfaces.Startup;

namespace Statistics.Commandline.Startup;

public interface ICommandlineStartupModule : IStartupModule
{
    /// <summary>
    ///     To be called during call to 'SetupApplication', wherein the application is configured.
    /// </summary>
    /// <param name="app"></param>
    void ConfigureApplication(IHostApplicationBuilder app);
}
