using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Statistics.Shared.Models.Settings;
using Statistics.Shared.Persistence;
using Statistics.Shared.Persistence.Core.Startup;
using Statistics.Shared.Startup;
using Statistics.Uno.Models;

namespace Statistics.Uno;

public class Startup : ModularStartup
{
    protected IHost? Host { get; private set; }

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

    /// <summary>
    /// TODO: Update to also work for deployment environment
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception">Throws exception if secrets file is missing</exception>
    private string GetConnectionString()
    {
        var secrets = Host?.Services.GetRequiredService<IOptions<SecretsConfig>>();
        return secrets?.Value.ConnectionString ?? "";
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        // Business Logic Services
    }

    /// <inheritdoc />
    public override void ConfigureApplication(IApplicationBuilder app)
    {
        app.Configure(host => host
#if DEBUG
            // Switch to Development environment when running in DEBUG
            .UseEnvironment(Environments.Development)
#endif
            .UseLogging(ConfigureLogging, enableUnoLogging: true).UseSerilog(consoleLoggingEnabled: true, fileLoggingEnabled: true)
            .UseConfiguration(configure: ConfigureConfigurationSource)
            .UseLocalization(ConfigureLocalization)
            );

        Host = app.Build();

        base.ConfigureApplication(app);

        Host = app.Build();
    }

    private void ConfigureLogging(HostBuilderContext context, ILoggingBuilder logBuilder)
    {
        // Configure log levels for different categories of logging
        logBuilder
            .SetMinimumLevel(
                context.HostingEnvironment.IsDevelopment() ?
                    LogLevel.Information :
                    LogLevel.Warning)

            // Default filters for core Uno Platform namespaces
            .CoreLogLevel(LogLevel.Warning);

        // Uno Platform namespace filter groups
        // Uncomment individual methods to see more detailed logging
        //// Generic Xaml events
        //logBuilder.XamlLogLevel(LogLevel.Debug);
        //// Layout specific messages
        //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
        //// Storage messages
        //logBuilder.StorageLogLevel(LogLevel.Debug);
        //// Binding related messages
        //logBuilder.XamlBindingLogLevel(LogLevel.Debug);
        //// Binder memory references tracking
        //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
        //// DevServer and HotReload related
        //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
        //// Debug JS interop
        logBuilder.WebAssemblyLogLevel(LogLevel.Debug);
    }

    private IHostBuilder ConfigureConfigurationSource(IConfigBuilder configBuilder)
    {
        return configBuilder.EmbeddedSource<App>().Section<AppConfig>().EmbeddedSource<App>("secrets").Section<SecretsConfig>();
    }

    private void ConfigureLocalization(HostBuilderContext context, IServiceCollection services)
    {
        // Enables localization (see appsettings.json for supported languages)
    }
}
