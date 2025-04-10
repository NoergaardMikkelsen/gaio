using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Statistics.Shared.Core.Newtonsoft.JsonConverters;
using IApplicationBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;

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
            options.SerializerSettings.Converters.AddRange([
                new StringEnumConverter(),
                new ArtificialIntelligenceJsonConverter(),
                new PromptJsonConverter(),
                new ResponseJsonConverter(),
                new ComplexSearchableJsonConverter(),
                new KeywordJsonConverter(),
            ]);
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
