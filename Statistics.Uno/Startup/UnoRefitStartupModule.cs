using Newtonsoft.Json;
using Statistics.Uno.Endpoints;
using Refit;
using Statistics.Shared.Abstraction.Interfaces.Refit;
using Statistics.Shared.Core.Newtonsoft;
using Statistics.Shared.Core.Newtonsoft.JsonConverters;
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
