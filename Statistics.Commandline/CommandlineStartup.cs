using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Statistics.Commandline.Startup;
using Statistics.Shared.Models.Settings;
using Statistics.Shared.Persistence;
using Statistics.Shared.Persistence.Core.Startup;
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
                throw new ArgumentNullException(nameof(secrets.ConnectionString),
                    "The connection string in the secrets was empty");

            options.UseNpgsql(secrets.ConnectionString);
            Console.WriteLine($"Connected to database with connection string '{secrets.ConnectionString}'");

#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        }));
    }

    private SecretsConfig GetSecrets()
    {
        SecretsConfig config = new SecretsConfig();
        Configuration.GetSection(nameof(SecretsConfig)).Bind(config);
        return config;
    }
}
