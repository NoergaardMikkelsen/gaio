using Newtonsoft.Json;
using Statistics.Uno.Endpoints;
using Refit;
using Statistics.Shared.Core.Newtonsoft;
using Statistics.Shared.Core.Newtonsoft.JsonConverters;

namespace Statistics.Uno.Startup;

public class UnoRefitStartupModule<TEndpoint> : IUnoStartupModule where TEndpoint : IRefitEndpoint
{
    private readonly string baseAddress;

    private static readonly RefitSettings settings = new()
    {
        ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings()
        {
            ContractResolver = new NoNavigationalPropertiesContractResolver(),
            Converters =
            {
                new ArtificialIntelligenceJsonConverter(),
                new PromptJsonConverter(),
                new ResponseJsonConverter(),
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
