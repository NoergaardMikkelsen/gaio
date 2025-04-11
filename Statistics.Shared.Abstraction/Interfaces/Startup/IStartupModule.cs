using Microsoft.Extensions.DependencyInjection;

namespace Statistics.Shared.Abstraction.Interfaces.Startup;

public interface IStartupModule
{
    /// <summary>
    ///     To be called during call to 'SetupServices', wherein different services are configured.
    /// </summary>
    /// <param name="services"></param>
    void ConfigureServices(IServiceCollection services);
}
