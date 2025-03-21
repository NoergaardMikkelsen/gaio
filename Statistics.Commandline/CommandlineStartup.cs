using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Statistics.Commandline.Startup;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Shared.Models.Settings;
using Statistics.Shared.Persistence;
using Statistics.Shared.Persistence.Core.Startup;
using Statistics.Shared.Persistence.Services;
using Statistics.Shared.Startup;

namespace Statistics.Commandline;

public class CommandlineStartup : CommandlineModularStartup
{
    public CommandlineStartup()
    {
        Console.WriteLine($"Constructing Startup Class...");

        AddModule(new CommandlineDatabaseContextStartupModule<StatisticsDatabaseContext>(options =>
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
            new CommandlineEntityQueryServiceStartupModule<ArtificialIntelligenceQueryService, ArtificialIntelligence,
                SearchableArtificialIntelligence>());
        AddModule(new CommandlineEntityQueryServiceStartupModule<PromptQueryService, Prompt, SearchablePrompt>());
        AddModule(new CommandlineEntityQueryServiceStartupModule<ResponseQueryService, Response, SearchableResponse>());
    }

    private SecretsConfig GetSecrets()
    {
        var config = new SecretsConfig();
        Configuration.GetSection(nameof(SecretsConfig)).Bind(config);
        return config;
    }

    /// <inheritdoc />
    public override void ConfigureApplication(IHostApplicationBuilder app)
    {
        base.ConfigureApplication(app);

        app.Configuration.AddJsonFile("appsettings.secrets.json", false, true);

        Configuration = app.Configuration;
    }
}
