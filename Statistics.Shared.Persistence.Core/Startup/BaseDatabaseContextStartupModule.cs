using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Statistics.Shared.Abstraction.Interfaces.Startup;

namespace Statistics.Shared.Persistence.Core.Startup;

public abstract class BaseDatabaseContextStartupModule<TContext> : IStartupModule where TContext : BaseDatabaseContext
{
    public delegate void SetupOptionsDelegate(DbContextOptionsBuilder options);

    protected readonly bool migrateOnStartup;
    private readonly SetupOptionsDelegate setupOptions;
    protected ILogger<BaseDatabaseContextStartupModule<TContext>>? logger;

    public BaseDatabaseContextStartupModule(SetupOptionsDelegate setup, bool migrateOnStartup = true)
    {
        if (typeof(TContext) is {IsAbstract: true,})
        {
            throw new ArgumentException($"Invalid type argument supplied to '{nameof(TContext)}'");
        }

        this.migrateOnStartup = migrateOnStartup;
        setupOptions = setup ?? throw new ArgumentNullException(nameof(setup));
    }


    public void ConfigureServices(IServiceCollection services)
    {
        logger = services.BuildServiceProvider().GetService<ILogger<BaseDatabaseContextStartupModule<TContext>>>();

        services.AddDbContext<TContext>(options => setupOptions.Invoke(options));

        logger?.LogDebug("Completed Configuration of Database Services.");
        Console.WriteLine("Completed Configuration of Database Services.");

        if (!migrateOnStartup)
        {
            return;
        }

        ServiceProvider? provider = services.BuildServiceProvider();
        using var context = provider.GetService<TContext>();
        context?.Database.Migrate();

        logger?.LogDebug("Completed Migration of Database.");
    }
}
