using Statistics.Shared.Abstraction.Interfaces.Startup;
using IApplicationBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;

namespace Statistics.Api.Startup;

public interface IApiStartupModule : IStartupModule
{
    /// <summary>
    ///     To be called during call to 'SetupApplication', wherein the application is configured.
    /// </summary>
    /// <param name="app"></param>
    void ConfigureApplication(IApplicationBuilder app);
}
