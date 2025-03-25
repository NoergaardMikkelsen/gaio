using Newtonsoft.Json;
using IApplicationBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;
using Newtonsoft.Json.Converters;

namespace Statistics.Api.Startup;

public class ApiStartupModule : IApiStartupModule
{
    private ILogger<ApiStartupModule>? logger;

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        logger = services.BuildServiceProvider().GetService<ILogger<ApiStartupModule>>();

        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });
        services.AddCors();

        services.AddEndpointsApiExplorer();

        logger?.LogDebug("Completed Configuration of Api Services.");
    }

    /// <inheritdoc />
    public void ConfigureApplication(IApplicationBuilder app)
    {
        if (app.GetType().FullName != typeof(WebApplication).FullName)
        {
            throw new InvalidOperationException(
                $"Expected application builder supplied to {nameof(ApiStartupModule)}.{nameof(ConfigureApplication)} to be of type {nameof(WebApplication)}, but it was of type '{app.GetType().FullName}'");
        }

        app.UseHttpsRedirection();
        app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(origin => true).AllowCredentials());
        app.UseAuthorization();

        var castApp = (WebApplication) app;
        castApp.MapControllers();

        logger?.LogDebug("Completed Configuration of Application.");
    }
}
