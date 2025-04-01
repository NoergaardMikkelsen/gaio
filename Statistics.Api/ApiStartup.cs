using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Statistics.Api.Startup;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Shared.Models.Settings;
using Statistics.Shared.Persistence;
using Statistics.Shared.Persistence.Services;
using IApplicationBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;

namespace Statistics.Api;

public class ApiStartup : ApiModularStartup
{
    private const string LOG_FILE = "Storage/satistics.log";

    public ApiStartup() : base()
    {
        Configuration = BuildConfiguration();

        AddModule(new LoggingStartupModule(LOG_FILE));

        AddModule(new ApiStartupModule());
        AddModule(new SwaggerStartupModule("AI Statistics Api"));

        AddModule(new ApiDatabaseContextStartupModule<StatisticsDatabaseContext>(options =>
        {
            SecretsConfig secrets = GetSecrets();

            if (string.IsNullOrEmpty(secrets.ConnectionString))
            {
                throw new ArgumentNullException(nameof(secrets.ConnectionString),
                    "The connection string in the secrets was empty");
            }

            options.UseNpgsql(secrets.ConnectionString);
            Console.WriteLine($"Connected to database with connection string '{secrets.ConnectionString}'");

#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        }));

        AddModule(
            new ApiEntityQueryServiceStartupModule<ArtificialIntelligenceQueryService, ArtificialIntelligence,
                SearchableArtificialIntelligence>());
        AddModule(new ApiEntityQueryServiceStartupModule<PromptQueryService, Prompt, SearchablePrompt>());
        AddModule(new ApiEntityQueryServiceStartupModule<ResponseQueryService, Response, SearchableResponse>());
    }

    private IConfiguration BuildConfiguration()
    {
        IConfigurationBuilder configBuilder = new ConfigurationBuilder();

        configBuilder.AddJsonFile("appsettings.json", false, true);
        configBuilder.AddJsonFile("appsettings.secrets.json", false, true);

        return configBuilder.Build();
    }

    private SecretsConfig GetSecrets()
    {
        var config = new SecretsConfig();
        Configuration.GetSection(nameof(SecretsConfig)).Bind(config);
        return config;
    }

    /// <inheritdoc />
    public override void ConfigureApplication(IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
        base.ConfigureApplication(app);
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
    }
}
