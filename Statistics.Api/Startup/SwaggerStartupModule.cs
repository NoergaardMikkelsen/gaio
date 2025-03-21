using Microsoft.OpenApi.Models;
using IApplicationBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;

namespace Statistics.Api.Startup;

public class SwaggerStartupModule : IApiStartupModule
{
    private readonly string apiTitle;
    private ILogger<SwaggerStartupModule>? logger;

    public SwaggerStartupModule(string apiTitle = "My API")
    {
        this.apiTitle = apiTitle;
    }

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        logger = services.BuildServiceProvider().GetService<ILogger<SwaggerStartupModule>>();

        // Register the Swagger generator, defining 1 or more Swagger documents.
        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = apiTitle, Version = "v1",}); });
        services.AddSwaggerGenNewtonsoftSupport();

        logger?.LogDebug("Completed Configuration of Swagger Services.");
    }

    /// <inheritdoc />
    public void ConfigureApplication(IApplicationBuilder app)
    {
        if (app.GetType().FullName != typeof(WebApplication).FullName)
        {
            throw new InvalidOperationException(
                $"Expected application builder supplied to {nameof(SwaggerStartupModule)}.{nameof(ConfigureApplication)} to be of type {nameof(WebApplication)}, but it was of type '{app.GetType().FullName}'");
        }

        var castApp = (WebApplication) app;
        if (!castApp.Environment.IsDevelopment())
        {
            return;
        }

        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger();

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
        // specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{apiTitle}");
            c.RoutePrefix = string.Empty;
        });

        logger?.LogDebug("Completed Configuration of Swagger Application.");
    }
}
