using Statistics.Shared.Abstraction.Interfaces.Refit;
using Statistics.Shared.Startup.Modules;

namespace Statistics.Uno.Startup;

public class UnoRefitStartupModule<TEndpoint> : RefitStartupModule<TEndpoint>, IUnoStartupModule
    where TEndpoint : IRefitEndpoint
{
    public UnoRefitStartupModule(string baseAddress) : base(baseAddress)
    {
    }

    /// <inheritdoc />
    public void ConfigureApplication(IApplicationBuilder app)
    {
    }
}
