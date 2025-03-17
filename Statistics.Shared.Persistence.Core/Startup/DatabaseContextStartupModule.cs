using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Statistics.Shared.Abstraction.Interfaces.Startup;

namespace Statistics.Shared.Persistence.Core.Startup;

public class DatabaseContextStartupModule<TContext> : IStartupModule where TContext : BaseDatabaseContext
{
    public delegate void SetupOptionsDelegate(DbContextOptionsBuilder options);

    private readonly bool migrateOnStartup;
    private readonly SetupOptionsDelegate setupOptions;
    private ILogger<DatabaseContextStartupModule<TContext>>? logger;


    public DatabaseContextStartupModule(SetupOptionsDelegate setup, bool migrateOnStartup = true)
    {
        if (typeof(TContext) is { IsAbstract: true, })
            throw new ArgumentException($"Invalid type argument supplied to '{nameof(TContext)}'");

        this.migrateOnStartup = migrateOnStartup;
        setupOptions = setup ?? throw new ArgumentNullException(nameof(setup));
    }


    public void ConfigureServices(IServiceCollection services)
    {
        logger = services.BuildServiceProvider().GetService<ILogger<DatabaseContextStartupModule<TContext>>>();

        services.AddDbContext<TContext>(options => setupOptions.Invoke(options));

        logger?.LogDebug("Completed Configuration of Database Services.");
    }

    public void ConfigureApplication(IApplicationBuilder app)
    {
        if (!migrateOnStartup)
            return;

        using IServiceScope service =
            app.Build().Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var context = service.ServiceProvider.GetService<TContext>();
        context?.Database.Migrate();

        logger?.LogDebug("Completed Configuration of Database Application.");
    }
}
