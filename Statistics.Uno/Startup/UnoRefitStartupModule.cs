using System.Text.Json;
using Statistics.Uno.Endpoints;
using Refit;
using Statistics.Uno.Startup.Converters;

namespace Statistics.Uno.Startup;

public class UnoRefitStartupModule<TEndpoint> : IUnoStartupModule where TEndpoint : IRefitEndpoint
{
    private readonly string baseAddress;
    private static readonly RefitSettings settings = new RefitSettings()
    {
        ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions()
        {
            Converters =
            {
                new ArtificialIntelligenceJsonConverter(),
                new PromptJsonConverter(),
                new ResponseJsonConverter(),
                new ResponseListJsonConverter(),
            },
        }),
    };

    public UnoRefitStartupModule(string baseAddress)
    {
        this.baseAddress = baseAddress;
    }

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        var api = RestService.For<TEndpoint>(baseAddress, settings);

        services.AddSingleton(typeof(TEndpoint), api);
    }

    /// <inheritdoc />
    public void ConfigureApplication(IApplicationBuilder app)
    {
    }

    private void ConfigureRefit(HostBuilderContext context, IServiceCollection services)
    {

    }
}
