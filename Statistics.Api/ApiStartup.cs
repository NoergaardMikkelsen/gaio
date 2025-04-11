using Microsoft.EntityFrameworkCore;
using Statistics.Api.Startup;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Shared.Models.Settings;
using Statistics.Shared.Persistence;
using Statistics.Shared.Persistence.Services;
using Statistics.Shared.Services.ArtificialIntelligence;
using IApplicationBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;

namespace Statistics.Api;

public class ApiStartup : ApiModularStartup
{
    private const string LOG_FILE = "Storage/satistics.log";
    private const string SECRETS_FILE = "appsettings.secrets.json";
    private const string APP_SETTINGS_FILE = "appsettings.json";
    private const string TEMPLATE_CONNECTION_STRING = "Your-Database-Connection-String-Here";

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
        AddModule(new ApiEntityQueryServiceStartupModule<KeywordQueryService, Keyword, SearchableKeyword>());
    }

    private IConfiguration BuildConfiguration()
    {
        EnsureSecretsFileExists();

        IConfigurationBuilder configBuilder = new ConfigurationBuilder();

        configBuilder.AddJsonFile(APP_SETTINGS_FILE, false, true);
        configBuilder.AddJsonFile(SECRETS_FILE, false, true);

        return configBuilder.Build();
    }

    private void EnsureSecretsFileExists()
    {
        if (File.Exists(SECRETS_FILE))
        {
            return;
        }

        var template = new
        {
            SecretsConfig = new SecretsConfig
            {
                ConnectionString = TEMPLATE_CONNECTION_STRING
            }
        };

        var templateContent = System.Text.Json.JsonSerializer.Serialize(template,
            new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

        Directory.CreateDirectory(Path.GetDirectoryName(SECRETS_FILE)!);
        File.WriteAllText(SECRETS_FILE, templateContent);

        throw new FileNotFoundException(
            $"Secrets file '{SECRETS_FILE}' was not found. An empty template secrets file as been generated. Please fill it out before running the application again.");
    }

    private SecretsConfig GetSecrets()
    {
        var config = new SecretsConfig();
        Configuration.GetSection(nameof(SecretsConfig)).Bind(config);

        if (string.IsNullOrEmpty(config.ConnectionString))
        {
            throw new ArgumentNullException(nameof(config.ConnectionString),
                "The connection string in the secrets was empty");
        }

        if (config.ConnectionString.Equals(TEMPLATE_CONNECTION_STRING, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new ArgumentException(
                $"The connection string in the secrets was not set. It is still the template value '{TEMPLATE_CONNECTION_STRING}'",
                nameof(config.ConnectionString));
        }

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

        services.AddTransient<IMasterArtificialIntelligencePromptService, MasterArtificialIntelligencePromptService>();
    }
}
