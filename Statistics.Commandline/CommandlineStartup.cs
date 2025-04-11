using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Statistics.Commandline.Startup;
using Statistics.Shared.Abstraction.Interfaces.Refit;
using Statistics.Shared.Models.Settings;
using Statistics.Shared.Persistence;

namespace Statistics.Commandline;

public class CommandlineStartup : CommandlineModularStartup
{
    public CommandlineStartup()
    {
        Console.WriteLine("Constructing Startup Class...");

        const string baseAddress = "https://localhost:7016/api/";

        AddModule(new CommandlineRefitStartupModule<IActionEndpoint>($"{baseAddress}Action"));

        // Kept to allow this project to be used for Migration generation.
        AddModule(new CommandlineDatabaseContextStartupModule<StatisticsDatabaseContext>(SetupDatabaseConnection));
    }

    private void SetupDatabaseConnection(DbContextOptionsBuilder options)
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
    }

    private SecretsConfig GetSecrets()
    {
        var config = new SecretsConfig();
        Configuration.GetSection(nameof(SecretsConfig)).Bind(config);
        return config;
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
    }

    /// <inheritdoc />
    public override void ConfigureApplication(IHostApplicationBuilder app)
    {
        base.ConfigureApplication(app);

        app.Configuration.AddJsonFile("appsettings.secrets.json", false, true);

        Configuration = app.Configuration;
    }
}
