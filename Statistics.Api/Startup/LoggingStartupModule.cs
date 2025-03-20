using Serilog;
using Serilog.Events;
using IApplicationBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;

namespace Statistics.Api.Startup;

public class LoggingStartupModule : IApiStartupModule
{
    private const string logPattern =
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u}] [{SourceContext}] {Message}{NewLine}{Exception}";

    private readonly string logPath;

    public LoggingStartupModule(string logPath)
    {
        this.logPath = logPath;
    }

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        var level = LogEventLevel.Information;
#if DEBUG
        level = LogEventLevel.Debug;
#endif

        Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console(outputTemplate: logPattern)
            .WriteTo.File(logPath, outputTemplate: logPattern, shared: true,
                flushToDiskInterval: TimeSpan.FromMinutes(1), restrictedToMinimumLevel: level,
                retainedFileCountLimit: 7, rollingInterval: RollingInterval.Day).CreateLogger();

        services.AddLogging(x => x.AddSerilog(Log.Logger));

        var logger = services.BuildServiceProvider().GetService<ILogger<LoggingStartupModule>>();
        logger?.LogDebug("Completed Configuration of Logging Services.");
    }

    /// <inheritdoc />
    public void ConfigureApplication(IApplicationBuilder app)
    {
    }
}
