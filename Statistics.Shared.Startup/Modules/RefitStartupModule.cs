using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Refit;
using Statistics.Shared.Abstraction.Interfaces.Refit;
using Statistics.Shared.Abstraction.Interfaces.Startup;
using Statistics.Shared.Core.Newtonsoft;
using Statistics.Shared.Core.Newtonsoft.JsonConverters;

namespace Statistics.Shared.Startup.Modules;

public class RefitStartupModule<TEndpoint> : IStartupModule where TEndpoint : IRefitEndpoint
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
                new ComplexSearchableJsonConverter(),
                new KeywordJsonConverter(),
            },
        }),
    };

    public RefitStartupModule(string baseAddress)
    {
        this.baseAddress = baseAddress;
    }

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        var api = RestService.For<TEndpoint>(baseAddress, settings);

        services.AddSingleton(typeof(TEndpoint), api);
    }
}
