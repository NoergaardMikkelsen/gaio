using Statistics.Api.Hubs;
using IApplicationBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;

namespace Statistics.Api.Startup;

public class ApiSignalrStartupModule : IApiStartupModule
{
    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSignalR();
    }

    /// <inheritdoc />
    public void ConfigureApplication(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapHub<NotificationHub>("/NotificationHub"); });
    }
}
