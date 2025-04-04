using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Shared.Services.Keywords;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Models;
using Statistics.Uno.Startup;

namespace Statistics.Uno;

public class UnoStartup : UnoModularStartup
{
    protected IHost? Host { get; private set; }

    public UnoStartup()
    {
        const string baseAddress = $"https://localhost:7016/";
        const string baseApiAddress = $"{baseAddress}api/";

        Console.WriteLine($"Constructing Startup Class...");

        AddModule(new UnoRefitStartupModule<IResponseEndpoint>($"{baseApiAddress}Response"));
        AddModule(new UnoRefitStartupModule<IArtificialIntelligenceEndpoint>(
            $"{baseApiAddress}ArtificialIntelligence"));
        AddModule(new UnoRefitStartupModule<IPromptEndpoint>($"{baseApiAddress}Prompt"));
        AddModule(new UnoRefitStartupModule<IKeywordEndpoint>($"{baseApiAddress}Keyword"));
        AddModule(new UnoRefitStartupModule<IActionEndpoint>($"{baseApiAddress}Action"));

        AddModule(new UnoSignalrStartupModule(() =>
            new HubConnectionBuilder().WithUrl($"{baseAddress}NotificationHub").Build()));
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddTransient<IAppliedKeywordService, AppliedKeywordService>();
    }

    /// <inheritdoc />
    public override void ConfigureApplication(IApplicationBuilder app)
    {
        app.Configure(host => host
#if DEBUG
            // Switch to Development environment when running in DEBUG
            .UseEnvironment(Environments.Development)
#endif
            .UseLogging(ConfigureLogging, true).UseSerilog(true, true)
            .UseConfiguration(configure: ConfigureConfigurationSource).UseLocalization(ConfigureLocalization)
            .UseSerialization(ConfigureSerialization));

        base.ConfigureApplication(app);

        Host = app.Build();
    }

    private void ConfigureSerialization(HostBuilderContext host, IServiceCollection services)
    {
        services.AddSingleton(new JsonSerializerOptions() {IncludeFields = true,});
    }

    private void ConfigureLogging(HostBuilderContext context, ILoggingBuilder logBuilder)
    {
        // Configure log levels for different categories of logging
        logBuilder.SetMinimumLevel(context.HostingEnvironment.IsDevelopment() ? LogLevel.Information : LogLevel.Warning)

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
        return configBuilder.EmbeddedSource<App>().Section<AppConfig>();
    }

    private void ConfigureLocalization(HostBuilderContext context, IServiceCollection services)
    {
        // Enables localization (see appsettings.json for supported languages)
    }
}
