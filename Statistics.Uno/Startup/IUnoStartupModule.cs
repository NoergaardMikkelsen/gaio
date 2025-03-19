using System.Reflection.Emit;
using Microsoft.Extensions.DependencyInjection;
using Statistics.Shared.Abstraction.Interfaces.Startup;

namespace Statistics.Uno.Startup;

public interface IUnoStartupModule : IStartupModule
{

    /// <summary>
    /// To be called during call to 'SetupApplication', wherein the application is configured.
    /// </summary>
    /// <param name="app"></param>
    void ConfigureApplication(IApplicationBuilder app);
}
