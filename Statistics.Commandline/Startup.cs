using Statistics.Shared.Persistence;
using Statistics.Shared.Persistence.Core.Startup;
using Statistics.Shared.Startup;

namespace Statistics.Commandline;

public class Startup : ModularStartup
{
    public Startup()
    {
        Console.WriteLine($"Constructing Startup Class...");

        AddModule(new DatabaseContextStartupModule<StatisticsDatabaseContext>(options =>
        {
            string connectionString = GetConnectionString();

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString),
                    "The connection string in the secrets was empty");

            options.UseNpgsql(connectionString);
            Console.WriteLine($"Connected to database with connection string '{connectionString}'");

#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        }));
    }
}
